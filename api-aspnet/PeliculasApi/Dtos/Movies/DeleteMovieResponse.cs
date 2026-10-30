namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Respuesta de DELETE /api/movies/:id: { id: 13, eliminado: true }
/// </summary>
public record DeleteMovieResponse(int Id, bool Eliminado);
