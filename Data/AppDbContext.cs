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
    public DbSet<RespuestaPropia> RespuestasPropias => Set<RespuestaPropia>();
    public DbSet<PerfilInfantil> PerfilesInfantiles => Set<PerfilInfantil>();
    public DbSet<ProgresoInfantil> ProgresosInfantiles => Set<ProgresoInfantil>();
    public DbSet<Lectura> Lecturas => Set<Lectura>();
    public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();

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

        // Una sola respuesta tuya por pregunta.
        modelBuilder.Entity<RespuestaPropia>().HasIndex(r => r.ClavePregunta).IsUnique();
        modelBuilder.Entity<RespuestaPropia>().Property(r => r.ClavePregunta).HasMaxLength(100);
        modelBuilder.Entity<RespuestaPropia>().Property(r => r.Texto).HasMaxLength(RespuestaPropia.LargoMaximo);

        // Modo infantil: un progreso por palabra, separado del progreso del adulto.
        modelBuilder.Entity<ProgresoInfantil>().HasIndex(p => p.PalabraId).IsUnique();
        modelBuilder.Entity<ProgresoInfantil>().Ignore(p => p.Aprendida);
        modelBuilder.Entity<PerfilInfantil>().Property(p => p.Nombre).HasMaxLength(PerfilInfantil.LargoMaximoNombre);

        // Lecturas: textos que pegas para leer y guardar palabras con su frase.
        modelBuilder.Entity<Lectura>().Property(l => l.Titulo).HasMaxLength(Lectura.LargoMaximoTitulo);
        modelBuilder.Entity<Lectura>().Property(l => l.Texto).HasMaxLength(Lectura.LargoMaximoTexto);
    }
}
