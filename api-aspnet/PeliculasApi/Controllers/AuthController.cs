using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using PeliculasApi.Dtos.Auth;
using PeliculasApi.Errors;
using PeliculasApi.Services;

namespace PeliculasApi.Controllers;

/// <summary>
/// Rutas de /api/auth, las mismas del AuthController de NestJS.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto) =>
        StatusCode(StatusCodes.Status201Created, await authService.RegisterAsync(dto));

    // 200 y no 201, igual que @HttpCode(HttpStatus.OK) en NestJS
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto dto) =>
        Ok(await authService.LoginAsync(dto));

    /// <summary>Devuelve el usuario del token: sirve para validar la sesion en los clientes.</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        // Con MapInboundClaims = false el claim conserva su nombre original "sub"
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(sub, out var userId))
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Unauthorized");
        }

        return await authService.MeAsync(userId);
    }
}
