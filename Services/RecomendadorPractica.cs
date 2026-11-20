namespace VocabularioIngles.Services;

/// <summary>Una actividad sugerida, con el porqué en una frase.</summary>
public record Recomendacion(string Titulo, string Motivo, string Icono, string Controlador, string Accion, string? Id = null);

/// <summary>
/// Elige UNA práctica para hoy, para no tener que decidir entre muchas opciones. Reglas, en orden:
/// 1) si fallas seguido 5 palabras o más, Mis errores; 2) en modo Trabajo, si no has empezado gramática, el tema de
/// errores típicos; 3) si no, una práctica en voz alta que cambia cada día (situaciones, shadowing o simulacro).
/// En modo Principiante solo aplica la primera regla.
/// </summary>
public static class RecomendadorPractica
{
    public const int MinimoErrores = 5;

    public static Recomendacion? Elegir(ModoApp modo, DateOnly hoy, int errores, int temasGramaticaEmpezados)
    {
        if (errores >= MinimoErrores)
        {
            return new Recomendacion("Mis errores",
                $"Tienes {errores} palabras que fallas seguido: repasarlas es lo que más rinde.",
                "diana", "Repaso", "Errores");
        }

        if (modo != ModoApp.Trabajo)
        {
            return null;
        }

        if (temasGramaticaEmpezados == 0)
        {
            return new Recomendacion("Gramática: errores típicos",
                "Empieza por los errores que más se notan en una entrevista.",
                "lapiz", "Gramatica", "Tema", "errores-tipicos");
        }

        return (hoy.DayNumber % 3) switch
        {
            0 => new Recomendacion("Situaciones guiadas",
                "Una conversación de trabajo en voz alta: stand-up, entrevista o code review.",
                "bandera", "Situaciones", "Index"),
            1 => new Recomendacion("Shadowing",
                "5 minutos repitiendo junto con el audio para sonar más natural.",
                "bocina", "Practica", "Shadowing"),
            _ => new Recomendacion("Simulacro de entrevista",
                "5 preguntas como en una entrevista real.",
                "maletin", "Simulacro", "Index")
        };
    }
}
