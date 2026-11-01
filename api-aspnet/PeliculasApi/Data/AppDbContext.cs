using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PeliculasApi.Models;

namespace PeliculasApi.Data;

/// <summary>
/// Puerta de entrada a la base de datos (lo que era PrismaService en NestJS).
/// Cada DbSet es una tabla.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<User> Users => Set<User>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite no guarda si una fecha es UTC: sin esto, al leerla se pierde y el
        // JSON sale sin la "Z" final ("2026-09-05T16:00:18" en vez de "...Z").
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(movie =>
        {
            movie.Property(m => m.Nombre).IsRequired();
            movie.Property(m => m.Imagen).IsRequired();
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Nombre).IsRequired();
            user.Property(u => u.Email).IsRequired();
            user.Property(u => u.Password).IsRequired();

            // Igual que email @unique en Prisma: dos usuarios no pueden tener el mismo correo
            user.HasIndex(u => u.Email).IsUnique();
        });
    }

    public override int SaveChanges()
    {
        AsignarFechas();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AsignarFechas();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Llena CreatedAt al crear y UpdatedAt al crear o editar,
    /// igual que @default(now()) y @updatedAt en Prisma.
    /// </summary>
    private void AsignarFechas()
    {
        var ahora = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Movie>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = ahora;
                entry.Entity.UpdatedAt = ahora;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = ahora;
            }
        }

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = ahora;
            }
        }
    }

    public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
}
