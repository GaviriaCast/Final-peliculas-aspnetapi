namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Respuesta de GET /api/movies: { data: [...], meta: { total, page, limit, totalPages } }
/// </summary>
public record PagedResponse<T>(IReadOnlyList<T> Data, PageMeta Meta);

public record PageMeta(int Total, int Page, int Limit, int TotalPages);
