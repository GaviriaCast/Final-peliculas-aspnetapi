using Microsoft.EntityFrameworkCore;
using PeliculasApi.Models;

namespace PeliculasApi.Data;

/// <summary>
/// Datos de ejemplo: las mismas 12 peliculas de backend/prisma/seed.ts.
/// EF Core lo ejecuta al aplicar las migraciones (UseSeeding / UseAsyncSeeding en Program.cs)
/// y solo inserta si la tabla esta vacia, para no duplicar datos en cada arranque.
/// </summary>
public static class SeedData
{
    public static void Seed(DbContext context, bool _)
    {
        var movies = context.Set<Movie>();
        if (movies.Any()) return;

        movies.AddRange(Peliculas());
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken cancellationToken)
    {
        var movies = context.Set<Movie>();
        if (await movies.AnyAsync(cancellationToken)) return;

        movies.AddRange(Peliculas());
        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<Movie> Peliculas() =>
    [
        new()
        {
            Nombre = "El Padrino",
            Imagen = "https://image.tmdb.org/t/p/w500/rPdtLWNsZmAtoZl9PK7S2wE3qiS.jpg",
            Director = "Francis Ford Coppola",
            Anio = 1972,
            Genero = "Drama",
            Sinopsis = "El patriarca de una dinastia del crimen organizado traspasa el control a su hijo menor.",
        },
        new()
        {
            Nombre = "Pulp Fiction",
            Imagen = "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
            Director = "Quentin Tarantino",
            Anio = 1994,
            Genero = "Crimen",
            Sinopsis = "Las vidas de dos sicarios, un boxeador y una pareja de atracadores se entrelazan.",
        },
        new()
        {
            Nombre = "Matrix",
            Imagen = "https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg",
            Director = "Lana y Lilly Wachowski",
            Anio = 1999,
            Genero = "Ciencia ficcion",
            Sinopsis = "Un programador descubre que la realidad es una simulacion controlada por maquinas.",
        },
        new()
        {
            Nombre = "Interestelar",
            Imagen = "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
            Director = "Christopher Nolan",
            Anio = 2014,
            Genero = "Ciencia ficcion",
            Sinopsis = "Un grupo de exploradores viaja por un agujero de gusano buscando un nuevo hogar.",
        },
        new()
        {
            Nombre = "Parasitos",
            Imagen = "https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
            Director = "Bong Joon-ho",
            Anio = 2019,
            Genero = "Thriller",
            Sinopsis = "Una familia humilde se infiltra poco a poco en la vida de una familia adinerada.",
        },
        new()
        {
            Nombre = "Ciudad de Dios",
            Imagen = "https://image.tmdb.org/t/p/w500/k7eYdWvhYQyRQoU2TB2A2Xu2TfD.jpg",
            Director = "Fernando Meirelles",
            Anio = 2002,
            Genero = "Crimen",
            Sinopsis = "Dos jovenes toman caminos opuestos en una favela de Rio de Janeiro.",
        },
        new()
        {
            Nombre = "Spirited Away",
            Imagen = "https://image.tmdb.org/t/p/w500/39wmItIWsg5sZMyRUHLkWBcuVCM.jpg",
            Director = "Hayao Miyazaki",
            Anio = 2001,
            Genero = "Animacion",
            Sinopsis = "Una nina queda atrapada en un mundo de espiritus y debe rescatar a sus padres.",
        },
        new()
        {
            Nombre = "El Senor de los Anillos: La Comunidad del Anillo",
            Imagen = "https://image.tmdb.org/t/p/w500/6oom5QYQ2yQTMJIbnvbkBL9cHo6.jpg",
            Director = "Peter Jackson",
            Anio = 2001,
            Genero = "Fantasia",
            Sinopsis = "Un hobbit emprende el viaje para destruir un anillo capaz de dominar la Tierra Media.",
        },
        new()
        {
            Nombre = "Whiplash",
            Imagen = "https://image.tmdb.org/t/p/w500/7fn624j5lj3xTme2SgiLCeuedmO.jpg",
            Director = "Damien Chazelle",
            Anio = 2014,
            Genero = "Drama",
            Sinopsis = "Un joven baterista es llevado al limite por un instructor implacable.",
        },
        new()
        {
            Nombre = "Coco",
            Imagen = "https://image.tmdb.org/t/p/w500/eKi8dIrr8voobbaGzDpe8w0PVbC.jpg",
            Director = "Lee Unkrich",
            Anio = 2017,
            Genero = "Animacion",
            Sinopsis = "Un nino viaja a la Tierra de los Muertos para descubrir el secreto de su familia.",
        },
        new()
        {
            Nombre = "Mad Max: Furia en la Carretera",
            Imagen = "https://image.tmdb.org/t/p/w500/hA2ple9q4qnwxp3hKVNhroipsir.jpg",
            Director = "George Miller",
            Anio = 2015,
            Genero = "Accion",
            Sinopsis = "En un desierto postapocaliptico, Max ayuda a un grupo de fugitivas a escapar.",
        },
        new()
        {
            Nombre = "El Laberinto del Fauno",
            Imagen = "https://image.tmdb.org/t/p/w500/67roafcTPUL8lm3ooEQfPVamvzj.jpg",
            Director = "Guillermo del Toro",
            Anio = 2006,
            Genero = "Fantasia",
            Sinopsis = "Una nina encuentra un mundo magico en plena posguerra civil espanola.",
        },
    ];
}
