using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>Qué se mide en una meta de la ruta.</summary>
public enum TipoMeta
{
    // Las palabras o frases de una categoría que ya aprendiste (acertadas en días distintos).
    Categoria,

    // Las frases de un tema de gramática que ya aprendiste.
    TemaGramatica,

    // Un sonido difícil dominado.
    Sonido,

    // Cuántos textos llevas en "Leer en contexto".
    Lecturas,

    // Cuántas respuestas de entrevista llevas escritas en "Mis respuestas".
    MisRespuestas,

    // Una práctica que la app no registra (situaciones, shadowing, simulacro): se sugiere, pero no se mide.
    Sugerida
}

/// <summary>
/// La ruta de aprendizaje: de cero a intermedio alto, en 5 etapas por nivel del MCER. Cada etapa junta lo que
/// conviene hacer en ese nivel para las cuatro habilidades (vocabulario, gramática, escuchar, hablar y leer), con
/// el contenido que ya existe en la app. No bloquea nada: solo dice qué sigue.
/// </summary>
public static class RutaAprendizaje
{
    /// <param name="Habilidad">Para qué sirve: Vocabulario, Gramática, Escuchar, Hablar o Leer.</param>
    /// <param name="Clave">Qué se mide: el nombre de la categoría, la clave del tema o del sonido, o cuántos textos.</param>
    /// <param name="Controlador">Solo en las sugeridas: a dónde lleva.</param>
    public record Meta(string Habilidad, string Texto, TipoMeta Tipo, string Clave = "",
        string? Controlador = null, string? Accion = null, string? Id = null);

    public record Etapa(int Numero, NivelMcer Nivel, string Titulo, string Descripcion, Meta[] Metas);

    private static Meta Vocabulario(string categoria) => new("Vocabulario", $"Palabras: {categoria}", TipoMeta.Categoria, categoria);
    private static Meta Frases(string categoria) => new("Vocabulario", $"Frases: {categoria}", TipoMeta.Categoria, categoria);

    private static Meta Gramatica(string clave) =>
        new("Gramática", $"Gramática: {TemasGramatica.Buscar(clave)!.Titulo}", TipoMeta.TemaGramatica, clave);

    private static Meta Sonido(string clave) =>
        new("Hablar", $"Sonido: {SonidosDificiles.Buscar(clave)!.Titulo}", TipoMeta.Sonido, clave);

    private static Meta Situacion(string clave) =>
        new("Hablar", $"Situación guiada: {SituacionesGuiadas.Buscar(clave)!.Titulo}", TipoMeta.Sugerida,
            Controlador: "Situaciones", Accion: "Ver", Id: clave);

    public static readonly Etapa[] Etapas =
    {
        new(1, NivelMcer.PreA1, "Desde cero",
            "Tus primeras palabras y los sonidos más fáciles de corregir. Con el audio de cada palabra empiezas a entrenar el oído.",
            new[]
            {
                Vocabulario("Colores"), Vocabulario("Números"), Vocabulario("Animales"), Vocabulario("Familia"),
                Vocabulario("El cuerpo"),
                Sonido("e-inicial"), Sonido("v-b"), Sonido("h")
            }),

        new(2, NivelMcer.A1, "Principiante",
            "Lo de todos los días (comida, casa, ropa, días) y los tres tiempos básicos: presente, pasado y futuro.",
            new[]
            {
                Vocabulario("Comida y bebida"), Vocabulario("La casa"), Vocabulario("La ropa"),
                Vocabulario("Días, meses y tiempo"), Vocabulario("Verbos básicos"),
                Gramatica("tiempos-basicos"),
                Sonido("i-corta"), Sonido("sh-ch"), Sonido("z"),
                new("Hablar", "Hablar en voz alta: di 5 frases y revisa tu pronunciación", TipoMeta.Sugerida,
                    Controlador: "Practica", Accion: "Index")
            }),

        new(3, NivelMcer.A2, "Básico",
            "La ciudad, los viajes y tu primer vocabulario de tecnología; la gramática básica y los sonidos más difíciles.",
            new[]
            {
                Vocabulario("La ciudad"), Vocabulario("Viajes"), Vocabulario("Salud"), Vocabulario("Verbos comunes"),
                Vocabulario("Tecnología"), Vocabulario("Trabajo"), Vocabulario("Verbos de programación"),
                Gramatica("errores-tipicos"), Gramatica("articulos"), Gramatica("preguntas"),
                Gramatica("preposiciones"), Gramatica("futuro"), Gramatica("cortesia"),
                Sonido("th-sorda"), Sonido("th-sonora"), Sonido("a-abierta"), Sonido("ed"),
                Situacion("standup"),
                new("Escuchar", "Shadowing: repite junto con el audio 5 minutos", TipoMeta.Sugerida,
                    Controlador: "Practica", Accion: "Shadowing")
            }),

        new(4, NivelMcer.B1, "Intermedio",
            "El inglés del trabajo: stand-up, code review, Git, reuniones y entrevista. Empiezas a leer textos reales.",
            new[]
            {
                Frases("Stand-up"), Frases("Code review"), Frases("Git y GitHub"), Frases("Agile y Scrum"),
                Frases("Reuniones y correo"), Frases("Entrevista"),
                Gramatica("presente-perfecto"), Gramatica("for-since"), Gramatica("pasiva"), Gramatica("condicional"),
                new("Leer", "Leer en contexto: 3 textos de tu trabajo", TipoMeta.Lecturas, "3"),
                Situacion("ayuda"), Situacion("codereview"), Situacion("entrevista"),
                new("Hablar", "Simulacro de entrevista", TipoMeta.Sugerida, Controlador: "Simulacro", Accion: "Index")
            }),

        new(5, NivelMcer.B2, "Intermedio alto",
            "La entrevista técnica en inglés: explicar conceptos, responder con tus palabras y leer documentación real.",
            new[]
            {
                Frases("Entrevista técnica"),
                new("Escribir", "Mis respuestas: escribe 5 respuestas de entrevista con tus datos", TipoMeta.MisRespuestas, "5"),
                new("Leer", "Leer en contexto: 10 textos de tu trabajo", TipoMeta.Lecturas, "10"),
                new("Hablar", "Simulacro de entrevista con tus propias respuestas", TipoMeta.Sugerida,
                    Controlador: "Simulacro", Accion: "Index")
            })
    };
}
