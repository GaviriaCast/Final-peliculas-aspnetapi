using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Parametros de la lista: ?search=matrix&amp;page=1&amp;limit=8
/// </summary>
public class QueryMoviesDto
{
    public const int DefaultPage = 1;
    public const int DefaultLimit = 8;

    [MaxLength(120, ErrorMessage = "La busqueda debe tener maximo 120 caracteres")]
    public string? Search { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "page debe ser un entero mayor o igual a 1")]
    public int? Page { get; set; }

    [Range(1, 50, ErrorMessage = "limit debe ser un entero entre 1 y 50")]
    public int? Limit { get; set; }
}
