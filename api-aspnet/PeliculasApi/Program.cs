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

builder.Services.AddControllers();

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Sin UseHttpsRedirection: Vue e Ionic llaman a la API por http://localhost:5080
app.UseAuthorization();

app.MapControllers();

app.Run();
