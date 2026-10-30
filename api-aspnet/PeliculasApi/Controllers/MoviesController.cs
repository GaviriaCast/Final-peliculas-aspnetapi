using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Dtos.Movies;
using PeliculasApi.Services;

namespace PeliculasApi.Controllers;

/// <summary>
/// Rutas de /api/movies, las mismas del MoviesController de NestJS.
/// [ApiController] valida los DTOs automaticamente y responde 400 si no cumplen.
/// </summary>
[ApiController]
[Route("api/movies")]
public class MoviesController(IMoviesService moviesService) : ControllerBase
{
    // --- Endpoints publicos (lectura) ---

    [HttpGet]
    public async Task<ActionResult<PagedResponse<MovieDto>>> FindAll([FromQuery] QueryMoviesDto query) =>
        await moviesService.FindAllAsync(query);

    // Sin restriccion {id:int}: asi un id no numerico responde 400 como ParseIntPipe, y no 404
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> FindOne(int id) =>
        await moviesService.FindOneAsync(id);

    // --- Endpoints de escritura (se protegen con [Authorize] en la parte de autenticacion) ---

    [HttpPost]
    public async Task<ActionResult<MovieDto>> Create(CreateMovieDto dto)
    {
        var movie = await moviesService.CreateAsync(dto);
        return CreatedAtAction(nameof(FindOne), new { id = movie.Id }, movie);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<MovieDto>> Update(int id, UpdateMovieDto dto) =>
        await moviesService.UpdateAsync(id, dto);

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteMovieResponse>> Remove(int id) =>
        await moviesService.RemoveAsync(id);
}
