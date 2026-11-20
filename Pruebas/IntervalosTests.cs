using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class IntervalosTests
{
    private static readonly DateTime Ahora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    // Historial sin fallos: la respuesta de ahora (acierto) y nada más.
    private static readonly bool[] SinFallos = { true };

    // Dos fallos en las últimas 5 respuestas: la palabra es difícil.
    private static readonly bool[] Dificil = { true, true, false, false };

    [Theory]
    [InlineData(0, 1, 1)] // un acierto ya no se repite el mismo día: pasa a mañana
    [InlineData(1, 2, 3)]
    [InlineData(2, 3, 7)]
    [InlineData(3, 4, 14)]
    [InlineData(4, 5, 30)]
    [InlineData(5, 5, 30)] // ya está en la etapa máxima: no sube más
    public void Fijos_un_acierto_sube_una_etapa(int etapa, int etapaEsperada, int diasEsperados)
    {
        var resultado = new IntervalosFijos().Calcular(etapa, true, Ahora, SinFallos);

        Assert.Equal(etapaEsperada, resultado.NuevaEtapa);
        Assert.Equal(Ahora.AddDays(diasEsperados), resultado.ProximoRepaso);
    }

    [Fact]
    public void Fijos_un_fallo_regresa_a_la_etapa_0_y_toca_ya()
    {
        var resultado = new IntervalosFijos().Calcular(5, false, Ahora, new[] { false });

        Assert.Equal(0, resultado.NuevaEtapa);
        Assert.Equal(Ahora, resultado.ProximoRepaso);
    }

    [Theory]
    [InlineData(0, 1)]  // 1 día → mínimo 1
    [InlineData(1, 1)]  // 3 días → 1
    [InlineData(2, 3)]  // 7 → 3
    [InlineData(3, 7)]  // 14 → 7
    [InlineData(4, 15)] // 30 → 15
    public void Adaptativos_una_palabra_dificil_espera_la_mitad(int etapa, int diasEsperados)
    {
        var resultado = new IntervalosAdaptativos().Calcular(etapa, true, Ahora, Dificil);

        Assert.Equal(etapa + 1, resultado.NuevaEtapa);
        Assert.Equal(Ahora.AddDays(diasEsperados), resultado.ProximoRepaso);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Ningun_acierto_se_repite_el_mismo_dia(int etapa)
    {
        foreach (var historial in new[] { SinFallos, Dificil })
        {
            Assert.True(new IntervalosFijos().Calcular(etapa, true, Ahora, historial).ProximoRepaso >= Ahora.AddDays(1));
            Assert.True(new IntervalosAdaptativos().Calcular(etapa, true, Ahora, historial).ProximoRepaso >= Ahora.AddDays(1));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Adaptativos_sin_fallos_es_igual_que_fijos(int etapa)
    {
        var fijos = new IntervalosFijos().Calcular(etapa, true, Ahora, SinFallos);
        var adaptativos = new IntervalosAdaptativos().Calcular(etapa, true, Ahora, SinFallos);

        Assert.Equal(fijos, adaptativos);
    }

    [Fact]
    public void Adaptativos_un_fallo_regresa_a_la_etapa_0()
    {
        var resultado = new IntervalosAdaptativos().Calcular(4, false, Ahora, new[] { false, false });

        Assert.Equal(new ResultadoAlgoritmo(0, Ahora), resultado);
    }

    [Theory]
    [InlineData(new[] { true }, false)]
    [InlineData(new[] { false }, false)]                            // 1 fallo no basta
    [InlineData(new[] { false, false }, true)]                      // 2 fallos
    [InlineData(new[] { true, true, true, false, false }, true)]    // los fallos siguen en la ventana de 5
    [InlineData(new[] { true, true, true, true, false, false }, false)] // uno ya salió de la ventana
    public void Una_palabra_es_dificil_con_2_fallos_en_las_ultimas_5(bool[] ultimas, bool esperado)
    {
        Assert.Equal(esperado, IntervalosAdaptativos.EsDificil(ultimas));
    }
}
