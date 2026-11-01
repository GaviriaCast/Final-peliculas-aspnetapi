using PeliculasApi.Models;

namespace PeliculasApi.Dtos.Auth;

/// <summary>
/// Usuario que se devuelve al cliente: { id, email, nombre }. Nunca incluye la contrasena.
/// </summary>
public record UserDto(int Id, string Email, string Nombre)
{
    public static UserDto FromEntity(User user) => new(user.Id, user.Email, user.Nombre);
}
