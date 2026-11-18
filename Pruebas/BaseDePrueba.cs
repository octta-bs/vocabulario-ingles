using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VocabularioIngles.Data;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

/// <summary>Un reloj que siempre marca la hora que se le diga.</summary>
public sealed class RelojFijo : TimeProvider
{
    private readonly DateTimeOffset _ahora;

    public RelojFijo(DateTime ahoraUtc)
    {
        _ahora = new DateTimeOffset(DateTime.SpecifyKind(ahoraUtc, DateTimeKind.Utc));
    }

    public override DateTimeOffset GetUtcNow() => _ahora;
}

/// <summary>
/// Una base SQLite en memoria creada con las migraciones reales y las palabras iniciales,
/// igual que la app al arrancar.
/// </summary>
public sealed class BaseDePrueba : IDisposable
{
    private readonly SqliteConnection _conexion;

    public AppDbContext Db { get; }

    public BaseDePrueba()
    {
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();
        Db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conexion).Options);
        Db.Database.Migrate();
        SeedData.Cargar(Db);
    }

    public static ZonaHorariaUsuario Zona(TimeProvider reloj)
    {
        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ZonaHoraria"] = "America/Mexico_City" })
            .Build();
        return new ZonaHorariaUsuario(configuracion, reloj);
    }

    public void Dispose()
    {
        Db.Dispose();
        _conexion.Dispose();
    }
}
