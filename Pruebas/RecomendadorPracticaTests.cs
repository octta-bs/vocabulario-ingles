using VocabularioIngles.Data;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class RecomendadorPracticaTests
{
    private static readonly DateOnly Hoy = new(2026, 11, 19);

    [Fact]
    public void Con_muchos_errores_recomienda_mis_errores_en_los_dos_modos()
    {
        foreach (var modo in new[] { ModoApp.Trabajo, ModoApp.Principiante })
        {
            var r = RecomendadorPractica.Elegir(modo, Hoy, errores: 5, temasGramaticaEmpezados: 3)!;
            Assert.Equal(("Repaso", "Errores"), (r.Controlador, r.Accion));
            Assert.Contains("5 palabras", r.Motivo);
        }
    }

    [Fact]
    public void Si_nunca_has_hecho_el_diagnostico_es_lo_primero_en_los_dos_modos()
    {
        foreach (var modo in new[] { ModoApp.Trabajo, ModoApp.Principiante })
        {
            var r = RecomendadorPractica.Elegir(modo, Hoy, errores: 9, temasGramaticaEmpezados: 0, diagnosticoHecho: false)!;
            Assert.Equal("Diagnostico", r.Controlador);
        }
    }

    [Fact]
    public void En_principiante_sin_errores_no_recomienda_nada()
    {
        Assert.Null(RecomendadorPractica.Elegir(ModoApp.Principiante, Hoy, errores: 4, temasGramaticaEmpezados: 0));
    }

    [Fact]
    public void En_trabajo_sin_gramatica_recomienda_el_tema_1()
    {
        var r = RecomendadorPractica.Elegir(ModoApp.Trabajo, Hoy, errores: 0, temasGramaticaEmpezados: 0)!;

        Assert.Equal(("Gramatica", "Tema", "tiempos-basicos"), (r.Controlador, r.Accion, r.Id));
        Assert.Equal(TemasGramatica.Todos[0].Clave, r.Id);
    }

    [Fact]
    public void Con_gramatica_empezada_cambia_la_practica_de_voz_cada_dia()
    {
        var tres = Enumerable.Range(0, 3)
            .Select(d => RecomendadorPractica.Elegir(ModoApp.Trabajo, Hoy.AddDays(d), 0, 1)!.Controlador)
            .ToList();

        Assert.Equal(new[] { "Practica", "Simulacro", "Situaciones" }.Order(), tres.Order());
        // El mismo día da siempre lo mismo.
        Assert.Equal(RecomendadorPractica.Elegir(ModoApp.Trabajo, Hoy, 0, 1), RecomendadorPractica.Elegir(ModoApp.Trabajo, Hoy, 0, 1));
    }
}
