using PeliculasApi.Models;

namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Pelicula tal como la devuelve la API. Las propiedades salen en camelCase
/// y en el mismo orden que en NestJS: id, nombre, imagen, ..., createdAt, updatedAt.
/// </summary>
public record MovieDto(
    int Id,
    string Nombre,
    string Imagen,
    string? Director,
    int? Anio,
    string? Genero,
    string? Sinopsis,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static MovieDto FromEntity(Movie movie) => new(
        movie.Id,
        movie.Nombre,
        movie.Imagen,
        movie.Director,
        movie.Anio,
        movie.Genero,
        movie.Sinopsis,
        movie.CreatedAt,
        movie.UpdatedAt);
}
