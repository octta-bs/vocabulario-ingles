namespace VocabularioIngles.Data;

/// <summary>
/// Preguntas típicas de entrevista para el simulacro, con una respuesta modelo corta y un consejo.
/// Escritas a mano (inglés de Estados Unidos): conviene revisarlas y adaptar las respuestas a tu experiencia.
/// </summary>
public static class PreguntasEntrevista
{
    public record Pregunta(string Ingles, string Espanol, string[] RespuestaModelo, string Consejo);

    public const int PorSimulacro = 5;

    // La primera siempre es "Tell me about yourself": casi todas las entrevistas empiezan así.
    public static readonly Pregunta[] Todas =
    {
        new("Tell me about yourself.", "Háblame de ti.",
            new[]
            {
                "I'm a software developer from Mexico.",
                "I have three years of experience with .NET.",
                "I mainly work with C#, ASP.NET Core and SQL Server.",
                "In my last project, I built a REST API for an online store."
            },
            "Responde en un minuto: quién eres, tu experiencia, tus tecnologías y un proyecto. Cambia los datos por los tuyos."),
        new("Why do you want to work with us?", "¿Por qué quieres trabajar con nosotros?",
            new[]
            {
                "I like your product and the technologies you use.",
                "I want to grow as a backend developer.",
                "I'm looking for a remote position with a good team."
            },
            "Menciona algo concreto de la empresa que hayas investigado."),
        new("What are your strengths?", "¿Cuáles son tus fortalezas?",
            new[]
            {
                "My strongest skill is backend development.",
                "I enjoy solving problems and learning new technologies.",
                "I work well in a team."
            },
            "Da un ejemplo real de cada fortaleza si te piden más detalle."),
        new("What is your biggest weakness?", "¿Cuál es tu mayor debilidad?",
            new[]
            {
                "My English is not perfect yet.",
                "I'm improving my English every day.",
                "I practice with an app and in meetings."
            },
            "Di una debilidad real y lo que haces para mejorarla."),
        new("What is dependency injection?", "¿Qué es la inyección de dependencias?",
            new[]
            {
                "A class receives its dependencies instead of creating them.",
                "In ASP.NET Core, I register services in Program.cs.",
                "Dependency injection makes the code easier to test."
            },
            "Si puedes, menciona un ejemplo de tu propio código."),
        new("How do you use async and await?", "¿Cómo usas async y await?",
            new[]
            {
                "I use async and await for operations that wait on the database.",
                "The thread is free while the operation runs.",
                "That way the app can handle more requests."
            },
            "Menciona operaciones de entrada y salida: base de datos, archivos o llamadas HTTP."),
        new("What is Entity Framework?", "¿Qué es Entity Framework?",
            new[]
            {
                "Entity Framework is an ORM for .NET.",
                "Entity Framework maps my classes to database tables.",
                "I use migrations to update the database schema."
            },
            "ORM significa Object-Relational Mapper: traduce entre objetos y tablas."),
        new("Tell me about a difficult bug you fixed.", "Cuéntame de un error difícil que hayas arreglado.",
            new[]
            {
                "The app was very slow in production.",
                "I found a query that loaded too much data.",
                "I fixed it, and the page was ten times faster."
            },
            "Usa el orden situación, acción y resultado. Cambia el ejemplo por uno tuyo."),
        new("How do you handle a disagreement in a code review?", "¿Cómo manejas un desacuerdo en un code review?",
            new[]
            {
                "I listen to the other person's point of view.",
                "I explain my reasons with examples.",
                "If we can't agree, we ask the team."
            },
            "Muestra que te importa más el código que tener la razón."),
        new("Do you have any questions for us?", "¿Tienes alguna pregunta para nosotros?",
            new[]
            {
                "Yes. What does a typical day look like in this role?",
                "How do you do code reviews in your team?",
                "What is the next step in the process?"
            },
            "Siempre ten dos o tres preguntas preparadas. Decir que no tienes ninguna da mala impresión.")
    };

    /// <summary>Las preguntas de un simulacro: primero "Tell me about yourself" y luego otras al azar, sin repetir.</summary>
    public static List<Pregunta> ElegirSimulacro(Random azar)
    {
        var resto = Todas.Skip(1).OrderBy(_ => azar.Next()).Take(PorSimulacro - 1);
        return new[] { Todas[0] }.Concat(resto).ToList();
    }
}
