using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using VocabularioIngles.Controllers;
using VocabularioIngles.Data;

namespace VocabularioIngles.Pruebas;

public class SituacionesTests
{
    [Fact]
    public void Hay_cuatro_situaciones_con_clave_unica()
    {
        Assert.Equal(new[] { "standup", "entrevista", "codereview", "ayuda" }, SituacionesGuiadas.Todas.Select(s => s.Clave));
    }

    [Fact]
    public void Cada_situacion_tiene_de_4_a_6_turnos_con_1_a_3_respuestas()
    {
        foreach (var situacion in SituacionesGuiadas.Todas)
        {
            Assert.InRange(situacion.Turnos.Length, 4, 6);
            Assert.All(situacion.Turnos, t => Assert.InRange(t.Respuestas.Length, 1, 3));
        }
    }

    [Fact]
    public void Ningun_texto_esta_vacio_y_el_ingles_no_tiene_acentos()
    {
        var turnos = SituacionesGuiadas.Todas.SelectMany(s => s.Turnos).ToList();
        var ingles = turnos.Select(t => t.Dice).Concat(turnos.SelectMany(t => t.Respuestas.Select(r => r.Ingles))).ToList();
        var espanol = turnos.Select(t => t.DiceEspanol).Concat(turnos.SelectMany(t => t.Respuestas.Select(r => r.Espanol))).ToList();

        Assert.All(ingles.Concat(espanol), texto => Assert.False(string.IsNullOrWhiteSpace(texto)));
        // Un acento o una "ñ" en el inglés sería una traducción en el lugar equivocado.
        Assert.All(ingles, texto => Assert.DoesNotMatch(new Regex("[áéíóúñ¿¡]"), texto));
    }

    [Fact]
    public void Las_respuestas_de_un_turno_no_se_repiten()
    {
        foreach (var turno in SituacionesGuiadas.Todas.SelectMany(s => s.Turnos))
        {
            Assert.Equal(turno.Respuestas.Length, turno.Respuestas.Select(r => r.Ingles).Distinct().Count());
        }
    }

    [Fact]
    public void Abrir_una_situacion_que_no_existe_da_404()
    {
        var controlador = new SituacionesController();

        Assert.IsType<NotFoundResult>(controlador.Ver("no-existe"));
        var vista = Assert.IsType<ViewResult>(controlador.Ver("standup"));
        Assert.Same(SituacionesGuiadas.Buscar("standup"), vista.Model);
    }
}
