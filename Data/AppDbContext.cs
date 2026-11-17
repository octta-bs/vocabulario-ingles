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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Una palabra tiene como máximo un progreso, y un progreso pertenece a una palabra.
        modelBuilder.Entity<ProgresoPalabra>()
            .HasOne(p => p.Palabra)
            .WithOne(p => p.Progreso)
            .HasForeignKey<ProgresoPalabra>(p => p.PalabraId);
    }
}
