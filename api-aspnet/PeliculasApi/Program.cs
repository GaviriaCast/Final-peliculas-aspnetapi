using Microsoft.EntityFrameworkCore;
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
        // No exponer al cliente el detalle interno del error de lectura del JSON
        options.AllowInputFormatterExceptionMessages = false;
    });

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
app.UseAuthorization();

app.MapControllers();

app.Run();
