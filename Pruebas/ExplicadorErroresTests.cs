using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class ExplicadorErroresTests
{
    private static readonly (string, string)[] Otras = { ("error", "error"), ("bug", "error (en un programa)"), ("server", "servidor") };

    private static Palabra Sustantivo(string ingles, string espanol) => new() { Ingles = ingles, Espanol = espanol, Tipo = TipoPalabra.Sustantivo };

    private static Palabra Verbo() => new()
    {
        Ingles = "fix", Espanol = "arreglar", Tipo = TipoPalabra.Verbo, Pasado = "fixed", Participio = "fixed",
        Ejemplo = "I fix bugs.", EjemploEs = "Arreglo errores."
    };

    private static Ejercicio Escribir(Palabra p) => new() { Palabra = p, Tipo = TipoEjercicio.EscribirPalabra, Respuesta = p.Ingles };

    private static string Una(Ejercicio ejercicio, string escrita) => Assert.Single(ExplicadorErrores.Explicar(ejercicio, escrita, Otras));

    [Fact]
    public void Tiempo_verbal_equivocado()
    {
        var pasado = new Ejercicio { Palabra = Verbo(), Tipo = TipoEjercicio.CompletarPasado, Respuesta = "fixed" };
        var futuro = new Ejercicio { Palabra = Verbo(), Tipo = TipoEjercicio.EscribirPalabra, Respuesta = "will fix" };

        Assert.Equal("Escribiste el presente (fix); aquí va el pasado (fixed).", Una(pasado, "fix"));
        Assert.Equal("Escribiste el pasado (fixed); aquí va el futuro (will fix).", Una(futuro, "fixed"));
    }

    [Theory]
    [InlineData("databse", "te faltó la \"a\"")]
    [InlineData("databasse", "te sobró una \"s\"")]
    [InlineData("datobase", "pusiste \"o\" en lugar de \"a\"")]
    [InlineData("dtaabase", "volteaste las letras \"at\"")]
    public void Una_letra_de_diferencia_dice_cual(string escrita, string esperado)
    {
        Assert.Equal($"Casi: {esperado}. Revisa la ortografía.", Una(Escribir(Sustantivo("database", "base de datos")), escrita));
    }

    [Fact]
    public void Plural()
    {
        Assert.Equal("Te sobró la \"s\" del final.", Una(Escribir(Sustantivo("bug", "error")), "bugs"));
        Assert.Equal("Te faltó la \"s\" del final.", Una(Escribir(Sustantivo("pants", "pantalones")), "pant"));
    }

    [Fact]
    public void Otra_palabra_de_la_app()
    {
        Assert.Equal("Escribiste \"error\" (error), que es otra palabra.", Una(Escribir(Sustantivo("bug", "error (en un programa)")), "error"));
    }

    [Fact]
    public void Respuesta_en_espanol()
    {
        Assert.Equal("Escribiste la palabra en español; aquí se pide en inglés.", Una(Escribir(Sustantivo("manager", "jefe, gerente")), "gerente"));
    }

    [Fact]
    public void Nada_escrito()
    {
        Assert.StartsWith("No escribiste nada.", Una(Escribir(Sustantivo("bug", "error")), "  "));
    }

    [Fact]
    public void En_frases_dice_que_falto_y_que_sobro()
    {
        var dictado = new Ejercicio
        {
            Palabra = Sustantivo("server", "servidor"), Tipo = TipoEjercicio.DictadoFrase, Respuesta = "The server is down."
        };

        var explicaciones = ExplicadorErrores.Explicar(dictado, "the server was down", Otras);

        Assert.Equal(new[] { "Te faltó: \"is\".", "Sobró: \"was\"." }, explicaciones);
        Assert.Equal("Tienes todas las palabras, pero en otro orden.", Una(dictado, "the server down is"));
    }

    [Fact]
    public void No_explica_si_esta_bien_ni_inventa_si_no_hay_regla()
    {
        var ejercicio = Escribir(Sustantivo("database", "base de datos"));

        Assert.Empty(ExplicadorErrores.Explicar(ejercicio, "Database.", Otras));
        Assert.Empty(ExplicadorErrores.Explicar(ejercicio, "keyboard", Otras));
    }

    [Fact]
    public void Una_forma_igual_en_dos_tiempos_no_se_explica_como_tiempo()
    {
        var leer = new Palabra { Ingles = "read", Espanol = "leer", Tipo = TipoPalabra.Verbo, Pasado = "read", Participio = "read" };
        var ejercicio = new Ejercicio { Palabra = leer, Tipo = TipoEjercicio.EscribirPalabra, Respuesta = "will read" };

        Assert.DoesNotContain(ExplicadorErrores.Explicar(ejercicio, "read", Otras), e => e.Contains("presente") || e.Contains("pasado"));
    }
}
