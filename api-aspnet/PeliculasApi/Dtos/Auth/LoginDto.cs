using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Dtos.Auth;

/// <summary>
/// Cuerpo de POST /api/auth/login. Sin MinLength: aqui no se crea una contrasena nueva.
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "El correo no es valido")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "La contrasena es obligatoria")]
    public string? Password { get; set; }
}
