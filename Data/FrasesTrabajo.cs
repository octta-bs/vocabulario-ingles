using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Frases para el trabajo y las entrevistas: stand-up, code review, entrevista, Git y GitHub,
/// Agile y Scrum, entrevista técnica, y reuniones y correo.
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
        },
        ["Git y GitHub"] = new FraseSemilla[]
        {
            new("I pushed my changes to the feature branch.", "Subí mis cambios a la rama de la funcionalidad.", "pushed"),
            new("There's a merge conflict in this file.", "Hay un conflicto de merge en este archivo.", "conflict"),
            new("Can you rebase your branch on main?", "¿Puedes hacer rebase de tu rama sobre main?", "rebase"),
            new("I created a new branch for this bug.", "Creé una rama nueva para este error.", "branch"),
            new("Don't forget to pull the latest changes.", "No olvides bajar los cambios más recientes.", "pull",
                "\"Pull\" es traer los cambios del repositorio remoto (git pull)."),
            new("I'll squash the commits before merging.", "Voy a juntar los commits en uno antes de hacer merge.", "squash"),
            new("The pipeline failed on the last commit.", "El pipeline falló en el último commit.", "failed"),
            new("Please write a clear commit message.", "Por favor, escribe un mensaje de commit claro.", "clear"),
            new("I reverted the commit that broke the build.", "Revertí el commit que rompió la compilación.", "reverted"),
            new("Could you review my pull request when you have time?", "¿Podrías revisar mi pull request cuando tengas tiempo?", "review"),
            new("I cloned the repository and ran the tests.", "Cloné el repositorio y corrí las pruebas.", "cloned"),
            new("The main branch is protected, so we need an approval.", "La rama main está protegida, así que necesitamos una aprobación.", "protected")
        },
        ["Agile y Scrum"] = new FraseSemilla[]
        {
            new("This user story has five points.", "Esta historia de usuario tiene cinco puntos.", "story"),
            new("Let's add it to the backlog.", "Agreguémoslo al backlog.", "backlog",
                "El backlog es la lista de pendientes del producto."),
            new("We'll demo it at the sprint review.", "Lo vamos a presentar en la sprint review.", "demo"),
            new("What is the priority of this ticket?", "¿Cuál es la prioridad de este ticket?", "priority"),
            new("The sprint ends next Friday.", "El sprint termina el próximo viernes.", "ends"),
            new("In the retrospective, we talked about what went well.", "En la retrospectiva hablamos de lo que salió bien.", "retrospective"),
            new("This task is too big, so we should break it down.", "Esta tarea es muy grande, así que deberíamos dividirla.", "break"),
            new("Who is the product owner for this feature?", "¿Quién es el product owner de esta funcionalidad?", "owner"),
            new("We can't commit to that in this sprint.", "No podemos comprometernos a eso en este sprint.", "commit",
                "Aquí \"commit\" es comprometerse, no un commit de Git."),
            new("What are the acceptance criteria?", "¿Cuáles son los criterios de aceptación?", "criteria"),
            new("I moved the ticket to In Progress.", "Moví el ticket a En progreso.", "moved"),
            new("Our velocity was lower this sprint.", "Nuestra velocidad fue más baja en este sprint.", "velocity")
        },
        ["Entrevista técnica"] = new FraseSemilla[]
        {
            new("Dependency injection makes the code easier to test.", "La inyección de dependencias hace que el código sea más fácil de probar.", "injection"),
            new("I use async and await for operations that wait on the database.", "Uso async y await en las operaciones que esperan a la base de datos.", "await"),
            new("Entity Framework maps my classes to database tables.", "Entity Framework mapea mis clases a tablas de la base de datos.", "maps"),
            new("A REST API uses HTTP methods like GET and POST.", "Una API REST usa métodos HTTP como GET y POST.", "methods"),
            new("The single responsibility principle says a class should have one reason to change.",
                "El principio de responsabilidad única dice que una clase debe tener una sola razón para cambiar.", "responsibility",
                "Es la S de SOLID."),
            new("An interface defines a contract without the implementation.", "Una interfaz define un contrato sin la implementación.", "contract"),
            new("I write unit tests with xUnit.", "Escribo pruebas unitarias con xUnit.", "unit"),
            new("LINQ lets me query collections in a readable way.", "LINQ me permite consultar colecciones de forma legible.", "query"),
            new("A struct is a value type, and a class is a reference type.", "Un struct es un tipo por valor y una clase es un tipo por referencia.", "value"),
            new("I use migrations to update the database schema.", "Uso migraciones para actualizar el esquema de la base de datos.", "migrations"),
            new("I would add caching to improve performance.", "Agregaría caché para mejorar el rendimiento.", "caching"),
            new("I'm not sure, but I would check the documentation.", "No estoy seguro, pero revisaría la documentación.", "sure",
                "Es mejor que inventar una respuesta en la entrevista.")
        },
        ["Reuniones y correo"] = new FraseSemilla[]
        {
            new("Sorry, you're on mute.", "Perdón, tienes el micrófono apagado.", "mute"),
            new("Could you share your screen?", "¿Podrías compartir tu pantalla?", "share"),
            new("Can you hear me okay?", "¿Me escuchas bien?", "hear"),
            new("I'll follow up by email.", "Te doy seguimiento por correo.", "follow"),
            new("Let's schedule a call for tomorrow.", "Agendemos una llamada para mañana.", "schedule"),
            new("Sorry, I didn't catch that. Could you say it again?", "Perdón, no te entendí. ¿Lo puedes repetir?", "catch"),
            new("Thanks for the update.", "Gracias por la actualización.", "update"),
            new("Please find the report attached.", "Te adjunto el reporte.", "attached",
                "Frase típica de correo para avisar que va un archivo adjunto."),
            new("I'll be out of the office on Monday.", "No voy a estar en la oficina el lunes.", "office"),
            new("Let's take this offline.", "Hablemos de esto aparte.", "offline",
                "Significa hablarlo después, fuera de la reunión; no tiene que ver con internet."),
            new("Your connection is a bit unstable.", "Tu conexión está un poco inestable.", "unstable"),
            new("Looking forward to hearing from you.", "Quedo en espera de tu respuesta.", "forward",
                "Cierre común de correo.")
        }
    };
}
