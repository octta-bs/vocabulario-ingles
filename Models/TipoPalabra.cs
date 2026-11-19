namespace VocabularioIngles.Models;

public enum TipoPalabra
{
    Sustantivo,
    Verbo,

    // Una frase completa de trabajo o de entrevista ("Today I'm working on the API.").
    Frase,

    // Una frase de un tema de gramática ("I have worked here for two years."), con su versión con el error
    // típico de hispanohablantes. Solo entra al repaso cuando eliges practicar su tema.
    Gramatica
}
