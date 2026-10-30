namespace PeliculasApi.Models;

/// <summary>
/// Misma entidad que el modelo Movie de Prisma en el backend de NestJS.
/// Los campos con "?" son opcionales (columnas que aceptan NULL).
/// </summary>
public class Movie
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Imagen { get; set; } = string.Empty;
    public string? Director { get; set; }
    public int? Anio { get; set; }
    public string? Genero { get; set; }
    public string? Sinopsis { get; set; }

    // Las llena AppDbContext al guardar (equivalente a @default(now()) y @updatedAt)
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
