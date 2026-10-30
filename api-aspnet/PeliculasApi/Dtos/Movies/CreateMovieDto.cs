using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Cuerpo de POST /api/movies. Mismas reglas que CreateMovieDto de NestJS (class-validator).
/// </summary>
public class CreateMovieDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(120, ErrorMessage = "El nombre debe tener maximo 120 caracteres")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "La imagen debe ser una URL valida")]
    [Url(ErrorMessage = "La imagen debe ser una URL valida")]
    public string? Imagen { get; set; }

    [MaxLength(120, ErrorMessage = "El director debe tener maximo 120 caracteres")]
    public string? Director { get; set; }

    [Range(1888, 2100, ErrorMessage = "El anio debe estar entre 1888 y 2100")]
    public int? Anio { get; set; }

    [MaxLength(60, ErrorMessage = "El genero debe tener maximo 60 caracteres")]
    public string? Genero { get; set; }

    [MaxLength(1000, ErrorMessage = "La sinopsis debe tener maximo 1000 caracteres")]
    public string? Sinopsis { get; set; }
}
