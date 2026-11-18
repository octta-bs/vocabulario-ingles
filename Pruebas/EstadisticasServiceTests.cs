using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class EstadisticasServiceTests : IDisposable
{
    // 18 de noviembre de 2026 a las 12:00 en la Ciudad de México (UTC-6).
    private static readonly DateTime Ahora = new(2026, 11, 18, 18, 0, 0, DateTimeKind.Utc);

    private readonly BaseDePrueba _base = new();
    private readonly EstadisticasService _estadisticas;

    public EstadisticasServiceTests()
    {
        _estadisticas = new EstadisticasService(_base.Db, BaseDePrueba.Zona(new RelojFijo(Ahora)));
        _base.Db.Configuraciones.Add(new Configuracion { MetaDiaria = 5 });
        _base.Db.SaveChanges();
    }

    public void Dispose() => _base.Dispose();

    /// <summary>Guarda respuestas un día (hora de México, al mediodía) de la palabra indicada.</summary>
    private void Responder(DateOnly dia, int aciertos, int fallos = 0, int palabraId = 1)
    {
        var mediodiaUtc = dia.ToDateTime(new TimeOnly(18, 0), DateTimeKind.Utc);
        for (var i = 0; i < aciertos + fallos; i++)
        {
            _base.Db.Respuestas.Add(new RegistroRespuesta
            {
                PalabraId = palabraId,
                Tipo = TipoEjercicio.EscribirPalabra,
                Acerto = i < aciertos,
                Fecha = mediodiaUtc.AddMinutes(i)
            });
        }

        _base.Db.SaveChanges();
    }

    private static DateOnly Dia(int dia) => new(2026, 11, dia);

    [Fact]
    public async Task Sin_respuestas_todo_esta_en_cero()
    {
        var resumen = await _estadisticas.ObtenerAsync();

        Assert.Equal(0, resumen.RachaActual);
        Assert.Equal(0, resumen.MejorRacha);
        Assert.Equal(0, resumen.TotalRespuestas);
        Assert.Null(resumen.PorcentajeTotal);
        Assert.Equal(EstadisticasService.DiasEnGrafica, resumen.UltimosDias.Count);
        Assert.Equal(Dia(18), resumen.UltimosDias[^1].Dia);
        Assert.Empty(resumen.MasFalladas);
    }

    [Fact]
    public async Task La_racha_cuenta_los_dias_seguidos_que_cumpliste_la_meta()
    {
        // Meta 5. Cumplidos: 11-14 (4 días seguidos), 15 no (2), 16-18 sí.
        foreach (var dia in new[] { 11, 12, 13, 14, 16, 17 })
        {
            Responder(Dia(dia), 5);
        }
        Responder(Dia(15), 2);
        Responder(Dia(18), 4, 2);

        var resumen = await _estadisticas.ObtenerAsync();

        Assert.True(resumen.HoyCumplida);
        Assert.Equal(3, resumen.RachaActual);
        Assert.Equal(4, resumen.MejorRacha);
    }

    [Fact]
    public async Task Si_hoy_aun_no_cumples_la_racha_cuenta_hasta_ayer()
    {
        Responder(Dia(16), 5);
        Responder(Dia(17), 5);
        Responder(Dia(18), 1);

        var resumen = await _estadisticas.ObtenerAsync();

        Assert.False(resumen.HoyCumplida);
        Assert.Equal(2, resumen.RachaActual);
    }

    [Fact]
    public async Task Si_ayer_no_cumpliste_la_racha_se_pierde()
    {
        Responder(Dia(15), 5);
        Responder(Dia(16), 5);

        Assert.Equal(0, (await _estadisticas.ObtenerAsync()).RachaActual);
    }

    [Fact]
    public async Task Los_dias_se_cuentan_con_la_hora_de_Mexico()
    {
        // 17 de noviembre a las 21:00 en México = 18 de noviembre a las 03:00 UTC: cuenta para el 17.
        _base.Db.Respuestas.Add(new RegistroRespuesta
        {
            PalabraId = 1, Tipo = TipoEjercicio.EscribirPalabra, Acerto = true,
            Fecha = new DateTime(2026, 11, 18, 3, 0, 0, DateTimeKind.Utc)
        });
        _base.Db.SaveChanges();

        var dias = (await _estadisticas.ObtenerAsync()).UltimosDias;

        Assert.Equal(1, dias.Single(d => d.Dia == Dia(17)).Ejercicios);
        Assert.Equal(0, dias.Single(d => d.Dia == Dia(18)).Ejercicios);
    }

    [Fact]
    public async Task Cuenta_ejercicios_y_aciertos_por_dia()
    {
        Responder(Dia(18), 3, 1);

        var resumen = await _estadisticas.ObtenerAsync();
        var hoy = resumen.UltimosDias[^1];

        Assert.Equal(new DiaDeProgreso(Dia(18), 4, 3), hoy);
        Assert.Equal(75, hoy.Porcentaje);
        Assert.Equal(75, resumen.PorcentajeTotal);
    }

    [Fact]
    public async Task Las_mas_falladas_van_primero_y_sin_las_que_nunca_fallaste()
    {
        Responder(Dia(18), 1, 3, palabraId: 2);
        Responder(Dia(18), 0, 1, palabraId: 3);
        Responder(Dia(18), 5, 0, palabraId: 4);

        var falladas = await _estadisticas.MasFalladasAsync(10);

        Assert.Equal(new[] { 2, 3 }, falladas.Select(f => f.Palabra.Id).ToArray());
        Assert.Equal((3, 1), (falladas[0].Fallos, falladas[0].Aciertos));
    }
}
