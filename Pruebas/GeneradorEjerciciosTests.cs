using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class GeneradorEjerciciosTests
{
    private readonly GeneradorEjercicios _generador = new();

    private static Palabra Frase() => new()
    {
        Id = 1,
        Tipo = TipoPalabra.Frase,
        Ingles = "Today I'm working on the new API.",
        Espanol = "Hoy estoy trabajando en la nueva API.",
        PalabraClave = "working"
    };

    private static Palabra Verbo() => new()
    {
        Id = 2,
        Tipo = TipoPalabra.Verbo,
        Ingles = "fix",
        Espanol = "arreglar",
        Pasado = "fixed",
        Participio = "fixed",
        Ejemplo = "I fix bugs.",
        EjemploEs = "Arreglo errores."
    };

    [Fact]
    public void Una_frase_tiene_traducir_completar_y_dictado()
    {
        var tipos = _generador.Posibles(Frase()).Select(e => e.Tipo).ToList();

        Assert.Equal(new[] { TipoEjercicio.EscribirPalabra, TipoEjercicio.CompletarFrase, TipoEjercicio.DictadoPalabra }, tipos);
    }

    [Fact]
    public void En_una_frase_el_hueco_va_en_la_palabra_clave()
    {
        var completar = _generador.Crear(Frase(), TipoEjercicio.CompletarFrase);

        Assert.Equal("Today I'm ___ on the new API.", completar.FraseConHueco);
        Assert.Equal("working", completar.Respuesta);
        Assert.Equal("Hoy estoy trabajando en la nueva API.", completar.FraseEs);
    }

    [Fact]
    public void Traducir_una_frase_pide_la_frase_completa()
    {
        var traducir = _generador.Crear(Frase(), TipoEjercicio.EscribirPalabra);

        Assert.Equal("Today I'm working on the new API.", traducir.Respuesta);
    }

    [Fact]
    public void Un_verbo_se_completa_en_presente_y_en_pasado()
    {
        var presente = _generador.Crear(Verbo(), TipoEjercicio.CompletarFrase);

        Assert.Equal("I ___ bugs.", presente.FraseConHueco);
        Assert.Equal("presente", presente.TiempoVerbal);

        // "fix" tiene frases de pasado y futuro en FrasesVerbos.cs.
        var pasado = _generador.Crear(Verbo(), TipoEjercicio.CompletarPasado);
        Assert.Equal(TipoEjercicio.CompletarPasado, pasado.Tipo);
        Assert.Equal("fixed", pasado.Respuesta);
    }

    [Fact]
    public void Si_el_tipo_no_se_puede_hacer_regresa_escribir_la_palabra()
    {
        // Una frase no tiene ejercicio de pasado.
        var ejercicio = _generador.Crear(Frase(), TipoEjercicio.CompletarPasado);

        Assert.Equal(TipoEjercicio.EscribirPalabra, ejercicio.Tipo);
    }

    [Fact]
    public void Una_palabra_que_solo_aparece_dentro_de_otra_no_arma_hueco()
    {
        // "fix" dentro de "fixed" no es la palabra completa: no hay ejercicio de completar.
        var palabra = Verbo();
        palabra.Ejemplo = "I fixed bugs.";

        Assert.DoesNotContain(_generador.Posibles(palabra), e => e.Tipo == TipoEjercicio.CompletarFrase);
    }

    [Fact]
    public void Elegir_siempre_da_un_ejercicio_valido()
    {
        var posibles = _generador.Posibles(Frase()).Select(e => e.Tipo).ToHashSet();

        for (var i = 0; i < 100; i++)
        {
            Assert.Contains(_generador.Elegir(Frase()).Tipo, posibles);
        }
    }
}
