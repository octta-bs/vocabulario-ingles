using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class PracticasTests : IDisposable
{
    private readonly BaseDePrueba _base = new();

    public void Dispose() => _base.Dispose();

    [Fact]
    public async Task Practicar_errores_califica_sin_cambiar_nada()
    {
        var servicio = new RepasoService(_base.Db, new IntervalosAdaptativos(), new GeneradorEjercicios(),
            new RitmoPorAciertos(), BaseDePrueba.Zona(TimeProvider.System));
        await servicio.AgregarNuevasAsync();
        var antes = _base.Db.Progresos.Single(p => p.PalabraId == 1);
        var (etapa, proximo) = (antes.Etapa, antes.ProximoRepaso);

        var bien = await servicio.EvaluarSinGuardarAsync(1, TipoEjercicio.EscribirPalabra, "Code");
        var mal = await servicio.EvaluarSinGuardarAsync(1, TipoEjercicio.EscribirPalabra, "cod");

        Assert.True(bien!.Acerto);
        Assert.False(mal!.Acerto);
        Assert.Empty(_base.Db.Respuestas);
        var despues = _base.Db.Progresos.Single(p => p.PalabraId == 1);
        Assert.Equal((etapa, proximo), (despues.Etapa, despues.ProximoRepaso));
    }

    [Fact]
    public void El_simulacro_empieza_con_tell_me_about_yourself_y_no_repite()
    {
        for (var semilla = 0; semilla < 20; semilla++)
        {
            var preguntas = PreguntasEntrevista.ElegirSimulacro(new Random(semilla));

            Assert.Equal(PreguntasEntrevista.PorSimulacro, preguntas.Count);
            Assert.Equal("Tell me about yourself.", preguntas[0].Ingles);
            Assert.Equal(preguntas.Count, preguntas.Select(p => p.Ingles).Distinct().Count());
        }
    }

    [Fact]
    public void Cada_pregunta_tiene_respuesta_modelo_y_consejo()
    {
        Assert.All(PreguntasEntrevista.Todas, p =>
        {
            Assert.False(string.IsNullOrWhiteSpace(p.Espanol));
            Assert.True(p.RespuestaModelo.Length >= 2);
            Assert.False(string.IsNullOrWhiteSpace(p.Consejo));
        });
    }
}
