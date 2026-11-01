using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PeliculasApi.Data;
using PeliculasApi.Errors;
using PeliculasApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---

// Base de datos SQLite con EF Core. El seed corre al aplicar las migraciones.
builder.Services.AddDbContext<AppDbContext>(options => options
    .UseSqlite(builder.Configuration.GetConnectionString("Default"))
    .UseSeeding(SeedData.Seed)
    .UseAsyncSeeding(SeedData.SeedAsync));

// Servicios de la aplicacion (inyeccion de dependencias). Scoped = una instancia por peticion.
builder.Services.AddScoped<IMoviesService, MoviesService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT: secreto (user-secrets) y duracion (appsettings.json), igual que JWT_SECRET y JWT_EXPIRES_IN
var jwtOptions = JwtOptions.Load(builder.Configuration);
builder.Services.AddSingleton(jwtOptions);

// Autenticacion JWT Bearer: lee "Authorization: Bearer <token>", verifica la firma y la expiracion.
// Equivale a JwtStrategy + JwtAuthGuard de NestJS.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Sin esto ASP.NET renombra "sub" a ClaimTypes.NameIdentifier y "email" a ClaimTypes.Email
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtOptions.SigningKey(),
            ValidateLifetime = true,
            // NestJS no valida emisor ni audiencia, y no da margen extra a la expiracion
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            // Como validate() en JwtStrategy: una firma valida no basta si el usuario ya no existe
            OnTokenValidated = async context =>
            {
                var sub = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                if (!int.TryParse(sub, out var userId) || !await db.Users.AnyAsync(u => u.Id == userId))
                {
                    context.HttpContext.Items["AuthError"] = "El usuario del token ya no existe";
                    context.Fail("El usuario del token ya no existe");
                }
            },

            // 401 con la forma { message, error, statusCode } cuando hay un mensaje propio.
            // Los demas 401 (sin token, vencido o alterado) los completa StatusCodeErrorWriter.
            OnChallenge = async context =>
            {
                if (context.HttpContext.Items["AuthError"] is string mensaje)
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        ErrorResponse.Create(StatusCodes.Status401Unauthorized, mensaje));
                }
            },
        };
    });
builder.Services.AddAuthorization();

// Errores con la forma de NestJS: { message, error, statusCode }
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddControllers(options =>
    {
        // Sin esto, un JSON mal formado agrega ademas "The dto field is required."
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

        // Mensajes en espanol cuando un parametro no se puede convertir (?page=abc, /movies/abc)
        var mensajes = options.ModelBindingMessageProvider;
        mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es valido para {campo.ToLowerInvariant()}");
        mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es valido, se esperaba un numero");
        mensajes.SetValueMustBeANumberAccessor(campo => $"{campo.ToLowerInvariant()} debe ser un numero");
        mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "El cuerpo de la peticion es obligatorio");
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // 400 con la forma de NestJS: { message: [...], error: "Bad Request", statusCode: 400 }
        options.InvalidModelStateResponseFactory = ValidationErrorFactory.Crear;
    })
    .AddJsonOptions(options =>
    {
        // Propiedades en camelCase (Nombre -> nombre, CreatedAt -> createdAt), como en NestJS.
        // Es el valor por defecto de ASP.NET, pero se deja explicito porque es parte del contrato.
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // No exponer al cliente el detalle interno del error de lectura del JSON
        options.AllowInputFormatterExceptionMessages = false;
    });

// CORS: los origenes de Vue (5173) e Ionic (8100, y los de Capacitor en el celular)
// salen de appsettings.json, igual que CORS_ORIGIN en el .env de NestJS
var origenesCors = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(origenesCors)
    .AllowAnyHeader()
    .AllowAnyMethod()));

// Documento OpenAPI en /openapi/v1.json (solo en desarrollo)
builder.Services.AddOpenApi();

var app = builder.Build();

// Crea peliculas.db y aplica las migraciones pendientes al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// --- Pipeline HTTP ---

app.UseExceptionHandler();

// Rutas que no existen y otros errores sin cuerpo: { message, error, statusCode }
app.UseStatusCodePages(StatusCodeErrorWriter.Escribir);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Sin UseHttpsRedirection: Vue e Ionic llaman a la API por http://localhost:5080
// CORS va antes de la autenticacion: asi un 401 tambien lleva las cabeceras de CORS
// y el cliente puede leerlo (si no, el navegador lo esconde como "error de red")
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
