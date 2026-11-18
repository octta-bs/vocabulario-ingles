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
    private static readonly int TotalGeneral = VocabularioGeneral.Categorias.Sum(c => c.Palabras.Length);
    private const int TotalInicial = 35;

    /// <summary>Mete al repaso las primeras palabras de Tecnología ("code", "bug"…), como antes de los niveles.</summary>
    private Task<int> AgregarTecnologiaAsync()
    {
        var tecnologia = _db.Categorias.Single(c => c.Nombre == "Tecnología");
        return _servicio.AgregarNuevasAsync(tecnologia.Id);
    }

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
        Assert.Equal(TotalInicial + TotalGeneral, _db.Palabras.Count(p => p.Tipo != TipoPalabra.Frase));
        Assert.Equal(TotalFrases, _db.Palabras.Count(p => p.Tipo == TipoPalabra.Frase));
    }

    [Fact]
    public void Cargar_dos_veces_no_duplica_nada()
    {
        SeedData.Cargar(_db);

        Assert.Equal(TotalInicial + TotalGeneral + TotalFrases, _db.Palabras.Count());
        Assert.Equal(3 + VocabularioGeneral.Categorias.Length + FrasesTrabajo.PorCategoria.Count, _db.Categorias.Count());
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
    public async Task En_orden_empieza_por_el_nivel_mas_basico()
    {
        var agregadas = await _servicio.AgregarNuevasAsync();

        var primeras = VocabularioGeneral.Categorias[0].Palabras.Take(5).Select(p => p.Ingles).ToArray();
        var enRepaso = _db.Progresos.Include(p => p.Palabra).ThenInclude(p => p!.Categoria)
            .OrderBy(p => p.PalabraId).ToList();
        Assert.Equal(5, agregadas);
        Assert.All(enRepaso, p => Assert.Equal(NivelMcer.PreA1, p.Palabra!.Categoria!.Nivel));
        Assert.Equal(primeras, enRepaso.Select(p => p.Palabra!.Ingles).ToArray());
    }

    [Fact]
    public async Task La_primera_categoria_es_la_que_de_verdad_sigue_en_orden()
    {
        var siguiente = (await _servicio.CategoriasConNuevasAsync())[0];

        await _servicio.AgregarNuevasAsync();

        Assert.All(_db.Progresos.Include(p => p.Palabra).ToList(), p => Assert.Equal(siguiente.Id, p.Palabra!.CategoriaId));
    }

    [Fact]
    public async Task Despues_de_Pre_A1_siguen_A1_y_luego_A2()
    {
        // Todo Pre A1 ya en repaso: lo siguiente debe ser A1.
        var preA1 = _db.Palabras.Where(p => p.Categoria!.Nivel == NivelMcer.PreA1).Select(p => p.Id).ToList();
        foreach (var id in preA1)
        {
            _db.Progresos.Add(new ProgresoPalabra { PalabraId = id, ProximoRepaso = DateTime.UtcNow.AddDays(30) });
        }
        await _db.SaveChangesAsync();

        await _servicio.AgregarNuevasAsync();

        var nuevas = _db.Progresos.Include(p => p.Palabra).ThenInclude(p => p!.Categoria)
            .Where(p => !preA1.Contains(p.PalabraId)).ToList();
        Assert.Equal(5, nuevas.Count);
        Assert.All(nuevas, p => Assert.Equal(NivelMcer.A1, p.Palabra!.Categoria!.Nivel));
    }

    [Fact]
    public async Task Las_categorias_con_palabras_nuevas_van_por_nivel()
    {
        var categorias = await _servicio.CategoriasConNuevasAsync();

        Assert.Equal(categorias.OrderBy(c => c.Nivel).Select(c => c.Id), categorias.Select(c => c.Id));
        Assert.Equal(VocabularioGeneral.Categorias[0].Nombre, categorias[0].Nombre);
        Assert.Equal(NivelMcer.B2, _db.Categorias.Single(c => c.Nombre == "Entrevista técnica").Nivel);
        Assert.Equal(NivelMcer.B1, _db.Categorias.Single(c => c.Nombre == "Stand-up").Nivel);
        Assert.Equal(NivelMcer.A2, _db.Categorias.Single(c => c.Nombre == "Tecnología").Nivel);
    }

    [Fact]
    public void El_vocabulario_general_es_valido()
    {
        var generador = new GeneradorEjercicios();
        var generales = _db.Palabras.Where(p => p.Categoria!.Nivel <= NivelMcer.A2 && p.Categoria.Nombre != "Tecnología"
                                               && p.Categoria.Nombre != "Trabajo" && p.Categoria.Nombre != "Verbos de programación")
            .ToList();

        Assert.Equal(TotalGeneral, generales.Count);
        Assert.All(generales, p =>
        {
            Assert.StartsWith("/", p.Pronunciacion);
            Assert.False(string.IsNullOrWhiteSpace(p.ComoSeLee));
            // Su ejemplo contiene la palabra completa, así que tiene ejercicio de completar.
            Assert.Contains(generador.Posibles(p), e => e.Tipo == TipoEjercicio.CompletarFrase);
            if (p.EsVerbo)
            {
                Assert.False(string.IsNullOrWhiteSpace(p.Pasado));
                Assert.False(string.IsNullOrWhiteSpace(p.Participio));
            }
        });
        Assert.Equal(generales.Count, generales.Select(p => p.Ingles.ToLowerInvariant()).Distinct().Count());
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
        await AgregarTecnologiaAsync();
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
        await AgregarTecnologiaAsync();

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
        await AgregarTecnologiaAsync();
        for (var i = 0; i < 10; i++)
        {
            await _servicio.ResponderAsync(1, TipoEjercicio.EscribirPalabra, "mal");
        }

        Assert.Equal(new RitmoNuevas(0, 0), await _servicio.CalcularRitmoAsync());
        Assert.Equal(0, await _servicio.AgregarNuevasAsync());
    }
}
