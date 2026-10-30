using Microsoft.EntityFrameworkCore;
using PeliculasApi.Data;
using PeliculasApi.Dtos.Movies;
using PeliculasApi.Errors;
using PeliculasApi.Models;

namespace PeliculasApi.Services;

/// <summary>
/// Logica del CRUD de peliculas (lo que era MoviesService en NestJS).
/// </summary>
public class MoviesService(AppDbContext db) : IMoviesService
{
    /// <summary>Listado con busqueda por nombre y paginacion.</summary>
    public async Task<PagedResponse<MovieDto>> FindAllAsync(QueryMoviesDto query)
    {
        var page = query.Page ?? QueryMoviesDto.DefaultPage;
        var limit = query.Limit ?? QueryMoviesDto.DefaultLimit;
        var search = query.Search?.Trim();

        IQueryable<Movie> movies = db.Movies.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
        {
            // LIKE en SQLite no distingue mayusculas para caracteres ASCII, igual que en Prisma.
            // No se usa Contains porque EF lo traduce a instr(), que si las distingue.
            var patron = $"%{EscaparLike(search)}%";
            movies = movies.Where(m => EF.Functions.Like(m.Nombre, patron, "\\"));
        }

        var total = await movies.CountAsync();

        // En long para que una pagina enorme (?page=999999999) no desborde el int
        var skip = (int)Math.Min((long)(page - 1) * limit, int.MaxValue);

        var data = await movies
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)limit));

        return new PagedResponse<MovieDto>(
            data.Select(MovieDto.FromEntity).ToList(),
            new PageMeta(total, page, limit, totalPages));
    }

    public async Task<MovieDto> FindOneAsync(int id)
    {
        var movie = await BuscarAsync(id);
        return MovieDto.FromEntity(movie);
    }

    public async Task<MovieDto> CreateAsync(CreateMovieDto dto)
    {
        var movie = new Movie
        {
            Nombre = dto.Nombre!,
            Imagen = dto.Imagen!,
            Director = dto.Director,
            Anio = dto.Anio,
            Genero = dto.Genero,
            Sinopsis = dto.Sinopsis,
        };

        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        return MovieDto.FromEntity(movie);
    }

    /// <summary>Solo cambia los campos que vinieron en el JSON.</summary>
    public async Task<MovieDto> UpdateAsync(int id, UpdateMovieDto dto)
    {
        var movie = await BuscarAsync(id);

        if (dto.Envio(nameof(dto.Nombre))) movie.Nombre = dto.Nombre!;
        if (dto.Envio(nameof(dto.Imagen))) movie.Imagen = dto.Imagen!;
        if (dto.Envio(nameof(dto.Director))) movie.Director = dto.Director;
        if (dto.Envio(nameof(dto.Anio))) movie.Anio = dto.Anio;
        if (dto.Envio(nameof(dto.Genero))) movie.Genero = dto.Genero;
        if (dto.Envio(nameof(dto.Sinopsis))) movie.Sinopsis = dto.Sinopsis;

        await db.SaveChangesAsync();

        return MovieDto.FromEntity(movie);
    }

    public async Task<DeleteMovieResponse> RemoveAsync(int id)
    {
        var movie = await BuscarAsync(id);

        db.Movies.Remove(movie);
        await db.SaveChangesAsync();

        return new DeleteMovieResponse(id, true);
    }

    private async Task<Movie> BuscarAsync(int id) =>
        await db.Movies.FindAsync(id)
        ?? throw new ApiException(StatusCodes.Status404NotFound, $"No existe la pelicula con id {id}");

    /// <summary>
    /// Escapa % y _ para que se busquen como texto y no como comodines de LIKE.
    /// </summary>
    private static string EscaparLike(string texto) =>
        texto.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
