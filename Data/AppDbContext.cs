using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Palabra> Palabras => Set<Palabra>();
    public DbSet<ProgresoPalabra> Progresos => Set<ProgresoPalabra>();
    public DbSet<RegistroRespuesta> Respuestas => Set<RegistroRespuesta>();
    public DbSet<Configuracion> Configuraciones => Set<Configuracion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Una palabra tiene como máximo un progreso, y un progreso pertenece a una palabra.
        modelBuilder.Entity<ProgresoPalabra>()
            .HasOne(p => p.Palabra)
            .WithOne(p => p.Progreso)
            .HasForeignKey<ProgresoPalabra>(p => p.PalabraId);

        // Se busca seguido por fecha (respuestas de hoy, las últimas N) y por palabra.
        modelBuilder.Entity<RegistroRespuesta>().HasIndex(r => r.Fecha);
        modelBuilder.Entity<RegistroRespuesta>().HasIndex(r => new { r.PalabraId, r.Fecha });
    }
}
