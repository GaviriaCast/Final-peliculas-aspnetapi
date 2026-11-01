using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PeliculasApi.Services;

/// <summary>
/// Configuracion del JWT (lo que eran JWT_SECRET y JWT_EXPIRES_IN en el .env de NestJS).
/// ExpiresInHours sale de appsettings.json; Secret sale de user-secrets y nunca va en el repo.
/// </summary>
public class JwtOptions
{
    public const string Section = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; } = 24;

    /// <summary>
    /// Lee la seccion "Jwt" y falla al arrancar si falta el secreto o es corto:
    /// HS256 exige una clave de al menos 32 bytes (si no, al firmar sale el error IDX10720).
    /// </summary>
    public static JwtOptions Load(IConfiguration configuration)
    {
        var options = configuration.GetSection(Section).Get<JwtOptions>() ?? new JwtOptions();

        if (Encoding.UTF8.GetByteCount(options.Secret) < 32)
        {
            throw new InvalidOperationException(
                "Falta el secreto del JWT o tiene menos de 32 caracteres. Configuralo con: " +
                "dotnet user-secrets set \"Jwt:Secret\" \"<un-secreto-de-32-caracteres-o-mas>\"");
        }

        return options;
    }

    public SymmetricSecurityKey SigningKey() => new(Encoding.UTF8.GetBytes(Secret));
}
