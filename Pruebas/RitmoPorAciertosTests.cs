using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class RitmoPorAciertosTests
{
    /// <summary>Arma un historial con tantos aciertos y fallos.</summary>
    private static List<bool> Respuestas(int aciertos, int fallos)
    {
        return Enumerable.Repeat(true, aciertos).Concat(Enumerable.Repeat(false, fallos)).ToList();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void Con_menos_de_10_respuestas_da_5_sin_porcentaje(int cuantas)
    {
        var ritmo = new RitmoPorAciertos().Calcular(Respuestas(0, cuantas));

        Assert.Equal(new RitmoNuevas(5, null), ritmo);
    }

    // Sobre las últimas 50 respuestas, cada acierto vale 2%.
    [Theory]
    [InlineData(50, 100, 8)]
    [InlineData(45, 90, 8)]
    [InlineData(44, 88, 5)]
    [InlineData(38, 76, 5)]
    [InlineData(37, 74, 3)]
    [InlineData(30, 60, 3)]
    [InlineData(29, 58, 0)]
    [InlineData(0, 0, 0)]
    public void La_cantidad_depende_del_porcentaje_de_aciertos(int aciertos, int porcentaje, int esperadas)
    {
        // 50 respuestas recientes y 50 fallos viejos, que no deben contar.
        var historial = Respuestas(aciertos, 50 - aciertos);
        historial.AddRange(Respuestas(0, 50));

        var ritmo = new RitmoPorAciertos().Calcular(historial);

        Assert.Equal(new RitmoNuevas(esperadas, porcentaje), ritmo);
    }

    [Fact]
    public void Solo_cuentan_las_ultimas_50()
    {
        // 50 aciertos recientes y muchos fallos viejos: 100%.
        var ritmo = new RitmoPorAciertos().Calcular(Respuestas(50, 200));

        Assert.Equal(new RitmoNuevas(8, 100), ritmo);
    }
}
