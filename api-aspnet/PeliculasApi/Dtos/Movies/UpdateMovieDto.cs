using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Dtos.Movies;

/// <summary>
/// Cuerpo de PATCH /api/movies/:id. Equivale a PartialType(CreateMovieDto) de NestJS:
/// todos los campos son opcionales y solo se actualizan los que llegan en el JSON.
///
/// Hay que distinguir "el campo no vino" de "vino en null": la app de Ionic manda
/// null para limpiar un campo opcional (por ejemplo, borrar el director).
/// El deserializador de JSON solo llama al setter de los campos que vienen, asi que
/// cada setter anota su nombre en <see cref="_enviados"/>.
/// </summary>
public class UpdateMovieDto : IValidatableObject
{
    private readonly HashSet<string> _enviados = [];

    private string? _nombre;
    private string? _imagen;
    private string? _director;
    private int? _anio;
    private string? _genero;
    private string? _sinopsis;

    [MaxLength(120, ErrorMessage = "El nombre debe tener maximo 120 caracteres")]
    public string? Nombre
    {
        get => _nombre;
        set { _nombre = value; _enviados.Add(nameof(Nombre)); }
    }

    [Url(ErrorMessage = "La imagen debe ser una URL valida")]
    public string? Imagen
    {
        get => _imagen;
        set { _imagen = value; _enviados.Add(nameof(Imagen)); }
    }

    [MaxLength(120, ErrorMessage = "El director debe tener maximo 120 caracteres")]
    public string? Director
    {
        get => _director;
        set { _director = value; _enviados.Add(nameof(Director)); }
    }

    [Range(1888, 2100, ErrorMessage = "El anio debe estar entre 1888 y 2100")]
    public int? Anio
    {
        get => _anio;
        set { _anio = value; _enviados.Add(nameof(Anio)); }
    }

    [MaxLength(60, ErrorMessage = "El genero debe tener maximo 60 caracteres")]
    public string? Genero
    {
        get => _genero;
        set { _genero = value; _enviados.Add(nameof(Genero)); }
    }

    [MaxLength(1000, ErrorMessage = "La sinopsis debe tener maximo 1000 caracteres")]
    public string? Sinopsis
    {
        get => _sinopsis;
        set { _sinopsis = value; _enviados.Add(nameof(Sinopsis)); }
    }

    /// <summary>True si el campo vino en el JSON (aunque sea null).</summary>
    public bool Envio(string campo) => _enviados.Contains(campo);

    /// <summary>
    /// Nombre e imagen son obligatorios en la base: se pueden cambiar, pero no vaciar.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Envio(nameof(Nombre)) && string.IsNullOrWhiteSpace(Nombre))
        {
            yield return new ValidationResult("El nombre es obligatorio", [nameof(Nombre)]);
        }

        if (Envio(nameof(Imagen)) && string.IsNullOrWhiteSpace(Imagen))
        {
            yield return new ValidationResult("La imagen debe ser una URL valida", [nameof(Imagen)]);
        }
    }
}
