using PeliculasApi.Dtos.Movies;

namespace PeliculasApi.Services;

/// <summary>
/// Contrato del servicio de peliculas. El controlador depende de esta interfaz
/// y no de la clase concreta: la implementacion la entrega la inyeccion de dependencias.
/// </summary>
public interface IMoviesService
{
    Task<PagedResponse<MovieDto>> FindAllAsync(QueryMoviesDto query);
    Task<MovieDto> FindOneAsync(int id);
    Task<MovieDto> CreateAsync(CreateMovieDto dto);
    Task<MovieDto> UpdateAsync(int id, UpdateMovieDto dto);
    Task<DeleteMovieResponse> RemoveAsync(int id);
}
