using Microsoft.EntityFrameworkCore;
using PeliculasApi.Data;
using PeliculasApi.Models;

namespace PeliculasApi.Repositories;

/// <summary>
/// Implementacion con Entity Framework Core: aqui estan todas las consultas a la base de datos de peliculas.
/// </summary>
public class MovieRepository(AppDbContext db) : IMovieRepository
{
    public async Task<(List<Movie> Items, int Total)> SearchAsync(string? search, int skip, int take)
    {
        IQueryable<Movie> movies = db.Movies.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
        {
            // LIKE en SQLite no distingue mayusculas para caracteres ASCII, igual que en Prisma.
            // No se usa Contains porque EF lo traduce a instr(), que si las distingue.
            var patron = $"%{EscaparLike(search)}%";
            movies = movies.Where(m => EF.Functions.Like(m.Nombre, patron, "\\"));
        }

        var total = await movies.CountAsync();

        var items = await movies
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Movie?> FindByIdAsync(int id) => await db.Movies.FindAsync(id);

    public async Task AddAsync(Movie movie)
    {
        db.Movies.Add(movie);
        await db.SaveChangesAsync();
    }

    /// <summary>La pelicula viene de FindByIdAsync, asi que EF ya sabe que campos cambiaron.</summary>
    public async Task UpdateAsync(Movie movie) => await db.SaveChangesAsync();

    public async Task RemoveAsync(Movie movie)
    {
        db.Movies.Remove(movie);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Escapa % y _ para que se busquen como texto y no como comodines de LIKE.
    /// </summary>
    private static string EscaparLike(string texto) =>
        texto.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
