var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---

builder.Services.AddControllers();

// Documento OpenAPI en /openapi/v1.json (solo en desarrollo)
builder.Services.AddOpenApi();

var app = builder.Build();

// --- Pipeline HTTP ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Sin UseHttpsRedirection: Vue e Ionic llaman a la API por http://localhost:5080
app.UseAuthorization();

app.MapControllers();

app.Run();
