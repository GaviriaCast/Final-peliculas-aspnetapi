using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Dtos.Auth;

/// <summary>
/// Cuerpo de POST /api/auth/register. Mismas reglas que RegisterDto de NestJS (class-validator).
/// </summary>
public class RegisterDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(80, ErrorMessage = "El nombre debe tener maximo 80 caracteres")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El correo no es valido")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string? Email { get; set; }

    // Maximo 72 porque BCrypt ignora todo lo que pase de 72 bytes
    [Required(ErrorMessage = "La contrasena debe tener al menos 6 caracteres")]
    [MinLength(6, ErrorMessage = "La contrasena debe tener al menos 6 caracteres")]
    [MaxLength(72, ErrorMessage = "La contrasena debe tener maximo 72 caracteres")]
    public string? Password { get; set; }
}
