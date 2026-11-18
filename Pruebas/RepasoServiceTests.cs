using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

/// <summary>
/// Prueba RepasoService contra una base SQLite en memoria creada con las migraciones reales
/// y las palabras iniciales, igual que la app al arrancar.
/// </summary>
public sealed class RepasoServiceTests : IDisposable
{
    private readonly SqliteConnection _conexion;
    private readonly AppDbContext _db;
    private readonly RepasoService _servicio;

    private static readonly int TotalFrases = FrasesTrabajo.PorCategoria.Values.Sum(f => f.Length);

    public RepasoServiceTests()
    {
        // La base en memoria vive mientras la conexión esté abierta.
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conexion).Options);
        _db.Database.Migrate();
        SeedData.Cargar(_db);

        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ZonaHoraria"] = "America/Mexico_City" })
            .Build();
        _servicio = new RepasoService(_db, new IntervalosAdaptativos(), new GeneradorEjercicios(),
            new RitmoPorAciertos(), new ZonaHorariaUsuario(configuracion, TimeProvider.System));
    }

    public void Dispose()
    {
        _db.Dispose();
        _conexion.Dispose();
    }

    [Fact]
    public void Las_migraciones_y_la_carga_inicial_dejan_palabras_y_frases()
    {
        Assert.Equal(35, _db.Palabras.Count(p => p.Tipo != TipoPalabra.Frase));
        Assert.Equal(TotalFrases, _db.Palabras.Count(p => p.Tipo == TipoPalabra.Frase));
    }

    [Fact]
    public void Cargar_dos_veces_no_duplica_nada()
    {
        SeedData.Cargar(_db);

        Assert.Equal(35 + TotalFrases, _db.Palabras.Count());
        Assert.Equal(3 + FrasesTrabajo.PorCategoria.Count, _db.Categorias.Count());
    }

    [Fact]
    public void Cada_frase_contiene_su_palabra_clave_completa()
    {
        // Si la palabra clave no aparece como palabra completa, la frase se queda sin ejercicio de completar.
        var generador = new GeneradorEjercicios();
        var sinHueco = _db.Palabras.Where(p => p.Tipo == TipoPalabra.Frase).AsEnumerable()
            .Where(p => generador.Posibles(p).All(e => e.Tipo != TipoEjercicio.CompletarFrase))
            .Select(p => p.Ingles)
            .ToList();

        Assert.Empty(sinHueco);
    }

    [Fact]
    public void No_hay_frases_repetidas()
    {
        var repetidas = FrasesTrabajo.PorCategoria.Values.SelectMany(f => f)
            .GroupBy(f => f.Ingles).Where(g => g.Count() > 1).Select(g => g.Key);

        Assert.Empty(repetidas);
    }

    [Fact]
    public async Task Al_principio_agrega_5_palabras_en_orden()
    {
        var agregadas = await _servicio.AgregarNuevasAsync();

        Assert.Equal(5, agregadas);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, _db.Progresos.Select(p => p.PalabraId).OrderBy(id => id).ToArray());
    }

    [Fact]
    public async Task Puede_agregar_solo_de_una_categoria()
    {
        var entrevista = _db.Categorias.Single(c => c.Nombre == "Entrevista");

        await _servicio.AgregarNuevasAsync(entrevista.Id);

        Assert.All(_db.Progresos.Include(p => p.Palabra), p => Assert.Equal(entrevista.Id, p.Palabra!.CategoriaId));
        Assert.Equal(5, _db.Progresos.Count());
    }

    [Fact]
    public async Task Cada_respuesta_se_guarda_y_cuenta_para_la_meta()
    {
        await _servicio.AgregarNuevasAsync();
        await _servicio.GuardarMetaDiariaAsync(6);

        await _servicio.ResponderAsync(1, TipoEjercicio.EscribirPalabra, "code");
        await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "mal");

        Assert.Equal(new[] { true, false }, _db.Respuestas.OrderBy(r => r.Id).Select(r => r.Acerto).ToArray());
        var meta = await _servicio.ObtenerMetaHoyAsync();
        Assert.Equal(new MetaDelDia(6, 2), meta);
        Assert.False(meta.Cumplida);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(500, 200)]
    [InlineData(40, 40)]
    public async Task La_meta_se_mantiene_entre_5_y_200(int pedida, int guardada)
    {
        await _servicio.GuardarMetaDiariaAsync(pedida);

        Assert.Equal(guardada, await _servicio.ObtenerMetaDiariaAsync());
    }

    [Fact]
    public async Task Una_palabra_que_fallas_seguido_vuelve_antes()
    {
        await _servicio.AgregarNuevasAsync();

        // "bug" (id 2): dos fallos y luego tres aciertos. En la etapa 3 lo normal son 3 días; difícil, 1.
        await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "mal");
        await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "mal");
        for (var i = 0; i < 3; i++)
        {
            await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "bug");
        }

        var progreso = _db.Progresos.Single(p => p.PalabraId == 2);
        Assert.Equal(3, progreso.Etapa);
        Assert.Equal(1, (progreso.ProximoRepaso - progreso.UltimoRepaso!.Value).TotalDays, precision: 3);
    }

    [Fact]
    public async Task Con_muchos_fallos_no_tocan_palabras_nuevas()
    {
        await _servicio.AgregarNuevasAsync();
        for (var i = 0; i < 10; i++)
        {
            await _servicio.ResponderAsync(1, TipoEjercicio.EscribirPalabra, "mal");
        }

        Assert.Equal(new RitmoNuevas(0, 0), await _servicio.CalcularRitmoAsync());
        Assert.Equal(0, await _servicio.AgregarNuevasAsync());
    }
}
