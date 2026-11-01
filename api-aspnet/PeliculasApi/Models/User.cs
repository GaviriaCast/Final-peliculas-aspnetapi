namespace PeliculasApi.Models;

/// <summary>
/// Mismo modelo User de Prisma en el backend de NestJS.
/// Password guarda solo el hash de BCrypt, nunca la contrasena en texto plano.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // La llena AppDbContext al guardar (equivalente a @default(now()) en Prisma)
    public DateTime CreatedAt { get; set; }
}
