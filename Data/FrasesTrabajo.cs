using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Frases para el trabajo y las entrevistas: stand-up, code review y entrevista.
/// Están escritas a mano (inglés de Estados Unidos) y conviene revisarlas.
/// Cada frase tiene una palabra clave, que es la que se quita en el ejercicio de completar.
/// Las de "Entrevista" con datos personales (años, tecnologías, proyecto) son un ejemplo:
/// cámbialas por los tuyos.
/// </summary>
public static class FrasesTrabajo
{
    public record FraseSemilla(string Ingles, string Espanol, string PalabraClave, string? Nota = null);

    public static readonly Dictionary<string, FraseSemilla[]> PorCategoria = new()
    {
        ["Stand-up"] = new FraseSemilla[]
        {
            new("Yesterday I fixed the login bug.", "Ayer arreglé el error del inicio de sesión.", "fixed"),
            new("Today I'm working on the new API.", "Hoy estoy trabajando en la nueva API.", "working"),
            new("I'm blocked by the database migration.", "Estoy bloqueado por la migración de la base de datos.", "blocked"),
            new("I don't have any blockers.", "No tengo ningún bloqueo.", "blockers",
                "Frase típica para cerrar tu turno en el stand-up."),
            new("I finished the task and opened a pull request.", "Terminé la tarea y abrí un pull request.", "finished"),
            new("I need help with the deployment.", "Necesito ayuda con el despliegue.", "help"),
            new("I'm still investigating the issue.", "Sigo investigando el problema.", "investigating"),
            new("I'll pair with a teammate on the payment feature.", "Voy a trabajar en pareja con un compañero en la función de pagos.", "pair",
                "\"Pair\" aquí es programar en pareja (pair programming)."),
            new("The tests are passing now.", "Las pruebas ya pasan.", "passing"),
            new("I should finish it by the end of the day.", "Debería terminarlo antes de que acabe el día.", "finish"),
            new("Can we discuss this after the stand-up?", "¿Podemos hablar de esto después del stand-up?", "discuss"),
            new("I'm waiting for feedback from the client.", "Estoy esperando comentarios del cliente.", "waiting")
        },
        ["Code review"] = new FraseSemilla[]
        {
            new("Could you add a unit test for this?", "¿Podrías agregar una prueba unitaria para esto?", "test"),
            new("Looks good to me.", "Me parece bien.", "good",
                "En los comentarios se abrevia LGTM."),
            new("Nice refactor, the code is much cleaner.", "Buena refactorización, el código quedó mucho más limpio.", "cleaner"),
            new("I left a few comments on your pull request.", "Dejé algunos comentarios en tu pull request.", "comments"),
            new("Can you rename this variable?", "¿Puedes cambiarle el nombre a esta variable?", "rename"),
            new("This method is too long, so let's split it.", "Este método es demasiado largo, así que vamos a dividirlo.", "split"),
            new("What happens if this value is null?", "¿Qué pasa si este valor es nulo?", "null"),
            new("I think we can remove this duplicate code.", "Creo que podemos quitar este código duplicado.", "duplicate"),
            new("Good catch, I'll fix it.", "Buena observación, lo corrijo.", "catch",
                "\"Good catch\" se usa cuando alguien encuentra un error que se te había pasado."),
            new("Please update the documentation too.", "Por favor, actualiza también la documentación.", "documentation"),
            new("I approved your changes.", "Aprobé tus cambios.", "approved"),
            new("This is a minor suggestion, not a blocker.", "Es una sugerencia menor, no algo que impida aprobarlo.", "suggestion")
        },
        ["Entrevista"] = new FraseSemilla[]
        {
            new("I'm a software developer from Mexico.", "Soy desarrollador de software de México.", "developer",
                "Inicio de tu respuesta a \"Tell me about yourself\"."),
            new("I have three years of experience with .NET.", "Tengo tres años de experiencia con .NET.", "experience",
                "\"Tell me about yourself\": cambia los años por los tuyos."),
            new("I mainly work with C#, ASP.NET Core and SQL Server.", "Trabajo principalmente con C#, ASP.NET Core y SQL Server.", "mainly",
                "\"Tell me about yourself\": cambia las tecnologías por las tuyas."),
            new("In my last project, I built a REST API for an online store.", "En mi último proyecto construí una API REST para una tienda en línea.", "built",
                "\"Tell me about yourself\": cambia el proyecto por uno tuyo."),
            new("I enjoy solving problems and learning new technologies.", "Me gusta resolver problemas y aprender tecnologías nuevas.", "solving"),
            new("My strongest skill is backend development.", "Mi mayor fortaleza es el desarrollo backend.", "skill"),
            new("I'm looking for a remote position.", "Busco un puesto remoto.", "remote"),
            new("I work well in a team.", "Trabajo bien en equipo.", "team"),
            new("I'm improving my English every day.", "Estoy mejorando mi inglés todos los días.", "improving"),
            new("Could you repeat the question, please?", "¿Podría repetir la pregunta, por favor?", "repeat"),
            new("That's a good question. Let me think.", "Buena pregunta. Déjeme pensarlo.", "think",
                "Sirve para ganar unos segundos antes de responder."),
            new("What does a typical day look like in this role?", "¿Cómo es un día normal en este puesto?", "typical",
                "Buena pregunta para hacerle tú al entrevistador al final.")
        }
    };
}
