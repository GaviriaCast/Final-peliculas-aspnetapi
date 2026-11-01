using System.Text.Json.Serialization;

namespace PeliculasApi.Dtos.Auth;

/// <summary>
/// Respuesta de register y login: { access_token, user }.
/// access_token va en snake_case porque asi lo devuelve NestJS y asi lo leen Vue e Ionic.
/// </summary>
public record AuthResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    UserDto User);
