using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PeliculasApi.Dtos.Auth;
using PeliculasApi.Errors;
using PeliculasApi.Models;
using PeliculasApi.Repositories;

namespace PeliculasApi.Services;

/// <summary>
/// Registro, login y usuario actual (lo que era AuthService en NestJS).
/// </summary>
public class AuthService(IUserRepository users, JwtOptions jwtOptions) : IAuthService
{
    // Mismas 10 rondas que bcrypt.hash(password, 10) en NestJS
    private const int SaltRounds = 10;

    /// <summary>Crea el usuario guardando SOLO el hash de la contrasena.</summary>
    public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email!;

        if (await users.EmailExistsAsync(email))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "Ese correo ya esta registrado");
        }

        var user = new User
        {
            Nombre = dto.Nombre!,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password!, SaltRounds),
        };

        // Dos registros con el mismo correo al mismo tiempo: el indice unico rechaza el segundo
        if (!await users.AddAsync(user))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "Ese correo ya esta registrado");
        }

        return BuildSession(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginDto dto)
    {
        var user = await users.FindByEmailAsync(dto.Email!);

        // Mismo mensaje si el correo no existe o si la contrasena esta mal,
        // para no revelar que correos estan registrados
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password!, user.Password))
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Credenciales invalidas");
        }

        return BuildSession(user);
    }

    public async Task<UserDto> MeAsync(int userId)
    {
        var user = await users.FindByIdAsync(userId)
            ?? throw new ApiException(StatusCodes.Status401Unauthorized, "El usuario del token ya no existe");

        return UserDto.FromEntity(user);
    }

    /// <summary>
    /// Firma el JWT con HS256 y devuelve el usuario sin la contrasena.
    /// El payload lleva lo mismo que en NestJS: sub (el id) y email.
    /// </summary>
    private AuthResponse BuildSession(User user)
    {
        var ahora = DateTime.UtcNow;

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id,
                [JwtRegisteredClaimNames.Email] = user.Email,
            },
            IssuedAt = ahora,
            Expires = ahora.AddHours(jwtOptions.ExpiresInHours),
            SigningCredentials = new SigningCredentials(jwtOptions.SigningKey(), SecurityAlgorithms.HmacSha256),
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);
        return new AuthResponse(accessToken, UserDto.FromEntity(user));
    }
}
