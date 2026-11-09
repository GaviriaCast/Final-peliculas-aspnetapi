using PeliculasApi.Models;

namespace PeliculasApi.Repositories;

/// <summary>
/// Acceso a la tabla Users. Lo usan AuthService y la validacion del JWT en Program.cs.
/// </summary>
public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(int id);
    Task<bool> ExistsAsync(int id);

    /// <summary>Guarda el usuario. Devuelve false si el indice unico rechazo el correo.</summary>
    Task<bool> AddAsync(User user);
}
