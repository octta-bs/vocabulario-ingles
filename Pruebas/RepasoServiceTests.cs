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
    private static readonly int TotalGramatica = TemasGramatica.Todos.Sum(t => t.Frases.Length);
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
        Assert.Equal(TotalInicial + TotalGeneral, _db.Palabras.Count(p => p.Tipo != TipoPalabra.Frase && p.Tipo != TipoPalabra.Gramatica));
        Assert.Equal(TotalFrases, _db.Palabras.Count(p => p.Tipo == TipoPalabra.Frase));
        Assert.Equal(TotalGramatica, _db.Palabras.Count(p => p.Tipo == TipoPalabra.Gramatica));
    }

    [Fact]
    public void Cargar_dos_veces_no_duplica_nada()
    {
        SeedData.Cargar(_db);

        Assert.Equal(TotalInicial + TotalGeneral + TotalFrases + TotalGramatica, _db.Palabras.Count());
        Assert.Equal(3 + VocabularioGeneral.Categorias.Length + FrasesTrabajo.PorCategoria.Count + TemasGramatica.Todos.Length,
            _db.Categorias.Count());
    }

    [Fact]
    public async Task Las_correcciones_del_contenido_llegan_a_una_base_que_ya_existe_sin_tocar_el_progreso()
    {
        // Una base vieja: textos de antes de la revisión, una palabra con progreso y una palabra propia.
        var three = _db.Palabras.Single(p => p.Ingles == "three");
        three.ComoSeLee = "zrí";
        var bug = _db.Palabras.Single(p => p.Ingles == "bug");
        bug.Ejemplo = "I fix a bug in the database.";
        _db.Progresos.Add(new ProgresoPalabra { PalabraId = bug.Id, Etapa = 3, Aciertos = 4, ProximoRepaso = DateTime.UtcNow });
        var propia = new Categoria { Nombre = LecturasService.NombreCategoria };
        _db.Palabras.Add(new Palabra { Categoria = propia, Ingles = "flaky", Espanol = "mi traducción", Ejemplo = "mi frase" });
        await _db.SaveChangesAsync();

        SeedData.Cargar(_db);
        _db.ChangeTracker.Clear();

        Assert.Equal("thrí", _db.Palabras.Single(p => p.Ingles == "three").ComoSeLee);
        Assert.Equal("There is a bug in the database.", _db.Palabras.Single(p => p.Ingles == "bug").Ejemplo);
        var progreso = _db.Progresos.Single(p => p.PalabraId == bug.Id);
        Assert.Equal((3, 4), (progreso.Etapa, progreso.Aciertos));
        var mia = _db.Palabras.Single(p => p.Ingles == "flaky");
        Assert.Equal(("mi traducción", "mi frase"), (mia.Espanol, mia.Ejemplo));
    }

    [Fact]
    public void Ningun_se_lee_empieza_con_e_antes_de_s_ni_usa_z_para_la_th()
    {
        var palabras = _db.Palabras.Where(p => p.ComoSeLee != "").ToList();

        // "escrín" enseñaría justo el error de agregar una e antes de s + consonante.
        Assert.DoesNotContain(palabras, p => p.Ingles.StartsWith('s') && p.ComoSeLee.StartsWith("es")
                                             && !p.Ingles.StartsWith("se") && !p.Ingles.StartsWith("si") && !p.Ingles.StartsWith("so")
                                             && !p.Ingles.StartsWith("su") && !p.Ingles.StartsWith("sa") && !p.Ingles.StartsWith("sy"));
        // La th sorda (/θ/) no se escribe con z: en México la z suena s.
        Assert.DoesNotContain(palabras, p => p.Pronunciacion.Contains('θ') && p.ComoSeLee.Contains('z'));
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
                                               && p.Categoria.Nombre != "Trabajo" && p.Categoria.Nombre != "Verbos de programación"
                                               && p.Tipo != TipoPalabra.Gramatica)
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
    public async Task Si_aciertas_cada_palabra_una_vez_la_sesion_termina_y_un_fallo_vuelve_al_final()
    {
        var agregadas = await AgregarTecnologiaAsync();
        Assert.True(agregadas >= 2);

        // La primera se falla; las demás se aciertan a la primera.
        var vistas = new List<int>();
        int? fallada = null;
        for (var i = 0; i < agregadas + 5; i++)
        {
            var progreso = await _servicio.SiguientePendienteAsync();
            if (progreso == null)
            {
                break;
            }

            vistas.Add(progreso.PalabraId);
            var correcta = progreso.Palabra!.Ingles;
            var respuesta = fallada == null ? "mal" : correcta;
            fallada ??= progreso.PalabraId;
            await _servicio.ResponderAsync(progreso.PalabraId, TipoEjercicio.EscribirPalabra, respuesta);
        }

        // Cada palabra salió una vez, más la fallada otra vez al final; después ya no queda nada para hoy.
        Assert.Equal(agregadas + 1, vistas.Count);
        Assert.Equal(agregadas, vistas.Distinct().Count());
        Assert.Equal(fallada, vistas[^1]);
        Assert.Null(await _servicio.SiguientePendienteAsync());
    }

    [Fact]
    public async Task Una_palabra_que_fallas_seguido_vuelve_antes()
    {
        await AgregarTecnologiaAsync();

        // "bug" (id 2): dos fallos y luego tres aciertos. En la etapa 3 lo normal son 7 días; difícil, 3.
        await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "mal");
        await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "mal");
        for (var i = 0; i < 3; i++)
        {
            await _servicio.ResponderAsync(2, TipoEjercicio.EscribirPalabra, "bug");
        }

        var progreso = _db.Progresos.Single(p => p.PalabraId == 2);
        Assert.Equal(3, progreso.Etapa);
        Assert.Equal(3, (progreso.ProximoRepaso - progreso.UltimoRepaso!.Value).TotalDays, precision: 3);
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
