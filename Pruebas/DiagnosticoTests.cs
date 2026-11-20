using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class DiagnosticoTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly DiagnosticoService _diagnostico;

    public DiagnosticoTests()
    {
        _diagnostico = new DiagnosticoService(_base.Db, new GeneradorEjercicios(), TimeProvider.System);
    }

    public void Dispose() => _base.Dispose();

    private static List<RespuestaDiagnostico> Responder(IEnumerable<PreguntaDiagnostico> preguntas, Func<PreguntaDiagnostico, bool> bien) =>
        preguntas.Select(p => new RespuestaDiagnostico(p.Ejercicio.Palabra.Id, p.Ejercicio.Tipo,
            bien(p) ? p.Ejercicio.Respuesta : "no sé")).ToList();

    [Fact]
    public async Task Arma_23_preguntas_de_facil_a_dificil_sin_repetir()
    {
        var preguntas = await _diagnostico.ArmarAsync(new Random(7));

        Assert.Equal(8, preguntas.Count(p => p.Area == AreaDiagnostico.Vocabulario));
        Assert.Equal(TemasGramatica.Todos.Length, preguntas.Count(p => p.Area == AreaDiagnostico.Gramatica));
        Assert.Equal(4, preguntas.Count(p => p.Area == AreaDiagnostico.Escucha));
        Assert.Equal(preguntas.Count, preguntas.Select(p => p.Ejercicio.Palabra.Id).Distinct().Count());
        Assert.DoesNotContain(preguntas, p => p.Ejercicio.Palabra.Categoria!.Nombre == LecturasService.NombreCategoria);

        foreach (var area in Enum.GetValues<AreaDiagnostico>())
        {
            var delArea = preguntas.Where(p => p.Area == area).ToList();
            // El área que se calcula al calificar es la misma con que se armó.
            Assert.All(delArea, p => Assert.Equal(area, DiagnosticoService.AreaDe(p.Ejercicio)));
            Assert.Equal(delArea.Select(p => p.Nivel).Order(), delArea.Select(p => p.Nivel));
        }
    }

    [Fact]
    public async Task Todo_bien_da_el_nivel_mas_alto_y_dice_que_falta_contenido()
    {
        var preguntas = await _diagnostico.ArmarAsync(new Random(1));
        var id = await _diagnostico.CalificarYGuardarAsync(Responder(preguntas, _ => true));

        var (diagnostico, resultado, anterior) = (await _diagnostico.ObtenerAsync(id!.Value))!.Value;
        Assert.Equal(preguntas.Count, diagnostico.Aciertos);
        Assert.Equal((NivelMcer?)NivelMcer.B2, diagnostico.NivelVocabulario);
        Assert.Equal((NivelMcer?)NivelMcer.B1, diagnostico.NivelGramatica);
        Assert.Equal((NivelMcer?)NivelMcer.B2, diagnostico.NivelEscucha);
        Assert.Empty(resultado.PuntosDebiles);
        Assert.Equal(3, resultado.LeFalta.Count);
        Assert.Contains(resultado.LeFalta, f => f.StartsWith("Gramática") && f.EndsWith("nivel B2."));
        Assert.Null(anterior);

        // No toca el repaso.
        Assert.Empty(_base.Db.Progresos);
        Assert.Empty(_base.Db.Respuestas);
    }

    [Fact]
    public async Task Todo_mal_no_da_nivel_y_sugiere_lo_mas_basico_primero()
    {
        var preguntas = await _diagnostico.ArmarAsync(new Random(2));
        var id = await _diagnostico.CalificarYGuardarAsync(Responder(preguntas, _ => false));

        var (diagnostico, resultado, _) = (await _diagnostico.ObtenerAsync(id!.Value))!.Value;
        Assert.Null(diagnostico.NivelVocabulario);
        Assert.Null(diagnostico.NivelGramatica);
        Assert.Null(diagnostico.NivelEscucha);
        Assert.Empty(resultado.LeFalta);
        Assert.Equal(3, resultado.PuntosDebiles.Count);
        // Pre A1 (vocabulario) va antes que A1 (escuchar) y que A2 (gramática).
        Assert.Equal("Repaso", resultado.PuntosDebiles[0].Controlador);
        Assert.Equal(TemasGramatica.Todos.Length, resultado.TemasFallados.Count);
    }

    [Fact]
    public async Task No_se_salta_niveles_y_el_segundo_diagnostico_se_compara_con_el_primero()
    {
        var preguntas = await _diagnostico.ArmarAsync(new Random(3));
        // Vocabulario: falla todo A1, pero acierta B1 y B2.
        var primero = await _diagnostico.CalificarYGuardarAsync(
            Responder(preguntas, p => !(p.Area == AreaDiagnostico.Vocabulario && p.Nivel == NivelMcer.A1)));
        var (d1, r1, _) = (await _diagnostico.ObtenerAsync(primero!.Value))!.Value;
        Assert.Equal((NivelMcer?)NivelMcer.PreA1, d1.NivelVocabulario);
        Assert.Contains(r1.PuntosDebiles, s => s.Texto.Contains("nivel A1"));

        var segundo = await _diagnostico.CalificarYGuardarAsync(Responder(preguntas, _ => true));
        var (_, _, anterior) = (await _diagnostico.ObtenerAsync(segundo!.Value))!.Value;
        Assert.Equal(primero, anterior!.Id);
        Assert.Equal(2, (await _diagnostico.HistorialAsync()).Count);
    }

    [Fact]
    public async Task Sin_respuestas_validas_no_guarda_nada()
    {
        Assert.Null(await _diagnostico.CalificarYGuardarAsync(new[] { new RespuestaDiagnostico(-1, TipoEjercicio.EscribirPalabra, "x") }));
        Assert.Empty(_base.Db.Diagnosticos);
    }
}
