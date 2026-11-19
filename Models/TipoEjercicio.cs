namespace VocabularioIngles.Models;

/// <summary>Las formas en que se puede preguntar una palabra durante el repaso.</summary>
public enum TipoEjercicio
{
    // Ves la palabra en español y la escribes en inglés.
    EscribirPalabra,

    // Completas el ejemplo de la palabra (para verbos, en presente).
    CompletarFrase,

    // Solo verbos: completas una frase con el verbo en pasado o en futuro.
    CompletarPasado,
    CompletarFuturo,

    // Dictado: oyes la palabra (o la frase de ejemplo) y la escribes.
    DictadoPalabra,
    DictadoFrase,

    // Solo gramática: ves la frase con el error y la escribes bien.
    CorregirFrase
}
