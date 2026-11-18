using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VocabularioIngles.Data;

/// <summary>
/// Deja la base de datos al día sin perder el progreso guardado:
/// 1. Si hay migraciones pendientes y el archivo ya existe, hace un respaldo con fecha.
/// 2. Si la base se creó antes con EnsureCreated (tiene tablas pero no historial de migraciones),
///    marca la primera migración ("InicioBase") como ya aplicada, porque esas tablas ya existen.
/// 3. Aplica las migraciones que falten.
/// </summary>
public static class PreparadorBaseDatos
{
    public static void Preparar(AppDbContext db, ILogger logger)
    {
        var pendientes = db.Database.GetPendingMigrations().ToList();
        if (pendientes.Count == 0)
        {
            return;
        }

        var archivo = new SqliteConnectionStringBuilder(db.Database.GetConnectionString()).DataSource;
        if (File.Exists(archivo) && new FileInfo(archivo).Length > 0)
        {
            var respaldo = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(archivo))!,
                $"{Path.GetFileNameWithoutExtension(archivo)}.respaldo-{DateTime.Now:yyyyMMdd-HHmmss}.db");

            // VACUUM INTO copia la base completa de forma segura aunque esté abierta.
            db.Database.ExecuteSql($"VACUUM INTO {respaldo}");
            logger.LogInformation("Respaldo de la base antes de migrar: {Respaldo}", respaldo);

            MarcarLineaBaseSiHaceFalta(db, logger);
        }

        db.Database.Migrate();
    }

    private static void MarcarLineaBaseSiHaceFalta(AppDbContext db, ILogger logger)
    {
        var lineaBase = db.Database.GetMigrations().First();
        if (db.Database.GetAppliedMigrations().Contains(lineaBase))
        {
            return;
        }

        var tieneTablas = db.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = 'Palabras'")
            .AsEnumerable()
            .Single() > 0;
        if (!tieneTablas)
        {
            return;
        }

        var historial = db.GetService<IHistoryRepository>();
        db.Database.ExecuteSqlRaw(historial.GetCreateIfNotExistsScript());
        db.Database.ExecuteSqlRaw(historial.GetInsertScript(new HistoryRow(lineaBase, ProductInfo.GetVersion())));
        logger.LogInformation("Base creada con EnsureCreated: se marcó {Migracion} como ya aplicada.", lineaBase);
    }
}
