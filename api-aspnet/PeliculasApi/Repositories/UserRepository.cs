using Microsoft.EntityFrameworkCore;
using PeliculasApi.Data;
using PeliculasApi.Models;

namespace PeliculasApi.Repositories;

/// <summary>
/// Implementacion con Entity Framework Core de las consultas de usuarios.
/// </summary>
public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string email) => db.Users.AnyAsync(u => u.Email == email);

    public Task<User?> FindByEmailAsync(string email) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> FindByIdAsync(int id) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> ExistsAsync(int id) => db.Users.AnyAsync(u => u.Id == id);

    public async Task<bool> AddAsync(User user)
    {
        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            // Dos registros con el mismo correo al mismo tiempo: el indice unico rechaza el segundo
            db.Entry(user).State = EntityState.Detached;
            return false;
        }
    }
}
