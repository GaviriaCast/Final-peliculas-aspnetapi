using PeliculasApi.Dtos.Auth;

namespace PeliculasApi.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto);
    Task<AuthResponse> LoginAsync(LoginDto dto);
    Task<UserDto> MeAsync(int userId);
}
