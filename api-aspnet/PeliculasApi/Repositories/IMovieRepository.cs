using PeliculasApi.Models;

namespace PeliculasApi.Repositories;

/// <summary>
/// Acceso a la tabla Movies. El servicio depende de esta interfaz y no de Entity Framework:
/// si se cambia la base de datos, solo se cambia la implementacion del repositorio.
/// </summary>
public interface IMovieRepository
{
    /// <summary>Peliculas cuyo nombre contiene el texto (o todas), mas recientes primero, con el total.</summary>
    Task<(List<Movie> Items, int Total)> SearchAsync(string? search, int skip, int take);
    Task<Movie?> FindByIdAsync(int id);
    Task AddAsync(Movie movie);
    Task UpdateAsync(Movie movie);
    Task RemoveAsync(Movie movie);
}
