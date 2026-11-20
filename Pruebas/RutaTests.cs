using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class RutaTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly SonidosService _sonidos;
    private readonly RutaService _ruta;

    public RutaTests()
    {
        _sonidos = new SonidosService(_base.Db, TimeProvider.System);
        _ruta = new RutaService(_base.Db, _sonidos);
    }

    public void Dispose() => _base.Dispose();

    private static IEnumerable<RutaAprendizaje.Meta> Metas(TipoMeta tipo) =>
        RutaAprendizaje.Etapas.SelectMany(e => e.Metas).Where(m => m.Tipo == tipo);

    [Fact]
    public void La_ruta_va_de_cero_a_b2_y_usa_todo_el_contenido_una_vez()
    {
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, RutaAprendizaje.Etapas.Select(e => e.Numero));
        var niveles = RutaAprendizaje.Etapas.Select(e => e.Nivel).ToList();
        Assert.Equal(niveles.Order(), niveles);
        Assert.Equal(NivelMcer.PreA1, niveles[0]);

        // Cada categoría de vocabulario y de frases, cada tema y cada sonido aparecen exactamente una vez.
        var categorias = VocabularioGeneral.Categorias.Select(c => c.Nombre)
            .Concat(FrasesTrabajo.PorCategoria.Keys)
            .Concat(new[] { "Tecnología", "Trabajo", "Verbos de programación" });
        Assert.Equal(categorias.Order(), Metas(TipoMeta.Categoria).Select(m => m.Clave).Order());
        Assert.Equal(TemasGramatica.Todos.Select(t => t.Clave).Order(), Metas(TipoMeta.TemaGramatica).Select(m => m.Clave).Order());
        Assert.Equal(SonidosDificiles.Todos.Select(s => s.Clave).Order(), Metas(TipoMeta.Sonido).Select(m => m.Clave).Order());

        // Dentro de la ruta, los temas y los sonidos van en su orden de estudio.
        Assert.Equal(TemasGramatica.Todos.Select(t => t.Clave), Metas(TipoMeta.TemaGramatica).Select(m => m.Clave));
        Assert.Equal(SonidosDificiles.Todos.Select(s => s.Clave), Metas(TipoMeta.Sonido).Select(m => m.Clave));
    }

    [Fact]
    public async Task Desde_cero_empieza_en_la_etapa_1_con_los_colores()
    {
        var ruta = await _ruta.CalcularAsync();

        Assert.Equal(1, ruta.Actual.Etapa.Numero);
        Assert.Equal(0, ruta.Actual.Avance);
        Assert.Equal("Colores", ruta.Siguiente!.Meta.Clave);
    }

    [Fact]
    public async Task Al_aprender_lo_de_la_etapa_1_se_completa_y_sigue_la_2()
    {
        var etapa1 = RutaAprendizaje.Etapas[0];
        var categorias = Metas(TipoMeta.Categoria).Take(5).Select(m => m.Clave).ToHashSet();
        foreach (var palabra in _base.Db.Palabras.Where(p => categorias.Contains(p.Categoria!.Nombre)))
        {
            _base.Db.Progresos.Add(new ProgresoPalabra { PalabraId = palabra.Id, Etapa = RutaService.EtapaAprendida, ProximoRepaso = DateTime.UtcNow.AddDays(3) });
        }
        await _base.Db.SaveChangesAsync();

        // Sin los sonidos, la etapa va en 5 de 8 metas: todavía no.
        var ruta = await _ruta.CalcularAsync();
        Assert.Equal(1, ruta.Actual.Etapa.Numero);
        Assert.Equal(5.0 / 8, ruta.Actual.Avance, precision: 3);
        Assert.Equal(TipoMeta.Sonido, ruta.Siguiente!.Meta.Tipo);

        // Con dos sonidos dominados llega al 80 %: pasa a la etapa 2.
        foreach (var clave in new[] { "e-inicial", "v-b" })
        {
            for (var i = 0; i < 10; i++)
            {
                await _sonidos.GuardarAsync(clave, EjercicioSonido.Escuchar, true);
            }
        }

        ruta = await _ruta.CalcularAsync();
        Assert.True(ruta.Etapas[0].Completa);
        Assert.Equal(2, ruta.Actual.Etapa.Numero);
        Assert.Equal(etapa1.Metas.Length, ruta.Etapas[0].Metas.Count);
    }

    [Fact]
    public async Task El_diagnostico_coloca_a_quien_ya_sabe()
    {
        // Vocabulario y escuchar en A2, gramática sin nivel (cuenta como A1): las etapas 1 y 2 quedan hechas.
        _base.Db.Diagnosticos.Add(new Diagnostico
        {
            Fecha = DateTime.UtcNow, NivelVocabulario = NivelMcer.A2, NivelEscucha = NivelMcer.A2, NivelGramatica = null
        });
        await _base.Db.SaveChangesAsync();

        var ruta = await _ruta.CalcularAsync();

        Assert.True(ruta.Etapas[0].PorDiagnostico && ruta.Etapas[1].PorDiagnostico);
        Assert.False(ruta.Etapas[2].PorDiagnostico);
        Assert.Equal(3, ruta.Actual.Etapa.Numero);
    }

    [Fact]
    public async Task Cada_meta_lleva_a_donde_se_practica()
    {
        var colores = (await _ruta.DestinoAsync(1, 0))!;
        Assert.Equal(("Repaso", "Nuevas", (ModoApp?)ModoApp.Principiante), (colores.Controlador, colores.Accion, colores.Modo));
        Assert.Equal(_base.Db.Categorias.Single(c => c.Nombre == "Colores").Id, colores.CategoriaNuevas);

        var standup = (await _ruta.DestinoAsync(4, 0))!;
        Assert.Equal((ModoApp?)ModoApp.Trabajo, standup.Modo);

        var tema = (await _ruta.DestinoAsync(2, 5))!;
        Assert.Equal(("Gramatica", "Tema", "tiempos-basicos"), (tema.Controlador, tema.Accion, tema.Id));

        var sonido = (await _ruta.DestinoAsync(1, 5))!;
        Assert.Equal(("Sonidos", "Ver", "e-inicial"), (sonido.Controlador, sonido.Accion, sonido.Id));

        Assert.Null(await _ruta.DestinoAsync(9, 0));
        Assert.Null(await _ruta.DestinoAsync(1, 99));
    }
}
