using System.Text.RegularExpressions;

namespace VocabularioIngles.Services;

/// <summary>Decide si lo que escribiste cuenta como la respuesta correcta.</summary>
public static class ComparadorRespuestas
{
    public static bool SonIguales(string? escrita, string? correcta)
    {
        return Normalizar(escrita) == Normalizar(correcta);
    }

    /// <summary>
    /// Deja el texto listo para comparar: sin mayúsculas, sin signos de puntuación y con un solo
    /// espacio entre palabras. Así en el dictado de frases no cuenta si pusiste el punto final.
    /// El apóstrofo se conserva (I'm, don't) pero se acepta también el "curvo" del celular, y las
    /// contracciones se expanden para que "I'm" y "I am" cuenten igual.
    /// </summary>
    public static string Normalizar(string? texto)
    {
        var limpio = (texto ?? "").ToLowerInvariant().Replace('’', '\'');
        limpio = Regex.Replace(limpio, @"[.,;:!?¿¡""“”]", " ");
        limpio = Regex.Replace(limpio, @"\s+", " ").Trim();

        // "I'm" y "I am" (o "don't" y "do not") cuentan igual.
        foreach (var (corta, larga) in Contracciones)
        {
            limpio = Regex.Replace(limpio, $@"(?<![\w']){Regex.Escape(corta)}(?![\w'])", larga);
        }

        return limpio;
    }

    private static readonly (string Corta, string Larga)[] Contracciones =
    {
        ("i'm", "i am"), ("you're", "you are"), ("we're", "we are"), ("they're", "they are"),
        ("it's", "it is"), ("that's", "that is"), ("what's", "what is"), ("there's", "there is"),
        ("let's", "let us"), ("i'll", "i will"), ("you'll", "you will"), ("we'll", "we will"),
        ("it'll", "it will"), ("i've", "i have"), ("we've", "we have"), ("i'd", "i would"),
        ("don't", "do not"), ("doesn't", "does not"), ("didn't", "did not"), ("isn't", "is not"),
        ("aren't", "are not"), ("wasn't", "was not"), ("weren't", "were not"), ("won't", "will not"),
        ("can't", "cannot"), ("can not", "cannot"), ("couldn't", "could not"), ("shouldn't", "should not"), ("haven't", "have not")
    };
}
