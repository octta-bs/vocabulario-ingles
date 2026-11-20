using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Situaciones guiadas (como el roleplay de Duolingo y Speak): diálogos cortos del trabajo en los que la app
/// dice su parte y tú respondes en voz alta con una de las respuestas modelo.
/// Están escritas a mano (inglés de Estados Unidos) y conviene revisarlas. Las de la entrevista son un
/// ejemplo: cámbialas por tus datos reales.
/// </summary>
public static class SituacionesGuiadas
{
    public record Respuesta(string Ingles, string Espanol);

    /// <summary>Un turno: lo que dice la otra persona y las respuestas que puedes dar (de 1 a 3).</summary>
    public record Turno(string Dice, string DiceEspanol, Respuesta[] Respuestas);

    public record Situacion(string Clave, string Titulo, string Descripcion, string Icono, NivelMcer Nivel, Turno[] Turnos);

    /// <summary>En orden de dificultad: del stand-up (frases cortas y fijas) a la entrevista.</summary>
    public static readonly Situacion[] Todas =
    {
        new("standup", "Daily stand-up", "Qué hiciste ayer, qué harás hoy y si tienes algún bloqueo.", "repaso", NivelMcer.A2, new Turno[]
        {
            new("Good morning! Let's start the stand-up. What did you work on yesterday?",
                "¡Buenos días! Empecemos el stand-up. ¿En qué trabajaste ayer?", new Respuesta[]
                {
                    new("Yesterday I fixed the login bug.", "Ayer arreglé el error del inicio de sesión."),
                    new("Yesterday I worked on the new API endpoint.", "Ayer trabajé en el nuevo endpoint de la API."),
                    new("Yesterday I finished the unit tests for the payment service.", "Ayer terminé las pruebas unitarias del servicio de pagos.")
                }),
            new("Nice. What are you working on today?", "Bien. ¿En qué vas a trabajar hoy?", new Respuesta[]
                {
                    new("Today I'm going to work on the search page.", "Hoy voy a trabajar en la página de búsqueda."),
                    new("Today I'll review the open pull requests.", "Hoy voy a revisar los pull requests abiertos."),
                    new("Today I'm working on the database migration.", "Hoy estoy trabajando en la migración de la base de datos.")
                }),
            new("Do you have any blockers?", "¿Tienes algún bloqueo?", new Respuesta[]
                {
                    new("No, I don't have any blockers.", "No, no tengo ningún bloqueo."),
                    new("Yes, I'm blocked by the API documentation.", "Sí, estoy bloqueado por la documentación de la API."),
                    new("I need access to the staging server.", "Necesito acceso al servidor de staging (pruebas).")
                }),
            new("Okay. When do you think it will be ready?", "Bien. ¿Cuándo crees que estará listo?", new Respuesta[]
                {
                    new("It should be ready by the end of the day.", "Debería estar listo antes de que acabe el día."),
                    new("I think I can finish it tomorrow.", "Creo que puedo terminarlo mañana."),
                    new("I'm not sure yet. I'll let you know this afternoon.", "Todavía no estoy seguro. Te aviso en la tarde.")
                }),
            new("Great, thanks. Anything else before we finish?", "Perfecto, gracias. ¿Algo más antes de terminar?", new Respuesta[]
                {
                    new("No, that's all from me. Thanks!", "No, es todo de mi parte. ¡Gracias!"),
                    new("Can we discuss the release after the stand-up?", "¿Podemos hablar del lanzamiento después del stand-up?")
                })
        }),

        new("ayuda", "Pedir ayuda con un bug", "Explicar el problema, qué ya probaste y pedir que te ayuden.", "diana", NivelMcer.B1, new Turno[]
        {
            new("Hi! What's up? You look busy.", "¡Hola! ¿Qué pasa? Te ves ocupado.", new Respuesta[]
                {
                    new("Hi! Do you have a minute? I need some help with a bug.", "¡Hola! ¿Tienes un minuto? Necesito ayuda con un bug."),
                    new("Hi! I'm stuck on an issue. Can you help me?", "¡Hola! Estoy atorado con un problema. ¿Me puedes ayudar?")
                }),
            new("Sure. What's the problem?", "Claro. ¿Cuál es el problema?", new Respuesta[]
                {
                    new("The app crashes when I save a new user.", "La aplicación se cae cuando guardo un usuario nuevo."),
                    new("The tests pass on my machine, but they fail in the pipeline.", "Las pruebas pasan en mi máquina, pero fallan en el pipeline."),
                    new("I get a null reference exception on the login page.", "Me sale una null reference exception en la página de inicio de sesión.")
                }),
            new("What have you tried so far?", "¿Qué has intentado hasta ahora?", new Respuesta[]
                {
                    new("I checked the logs and debugged the code.", "Revisé los logs y depuré el código."),
                    new("I tried to reproduce it, but I couldn't.", "Intenté reproducirlo, pero no pude."),
                    new("I searched for the error, but nothing worked.", "Busqué el error, pero nada funcionó.")
                }),
            new("Can you share your screen so I can see it?", "¿Puedes compartir tu pantalla para que lo vea?", new Respuesta[]
                {
                    new("Sure, let me share my screen.", "Claro, déjame compartir mi pantalla."),
                    new("Yes, give me one second.", "Sí, dame un segundo.")
                }),
            new("I think the problem is the connection string. Try changing it.",
                "Creo que el problema es la cadena de conexión. Intenta cambiarla.", new Respuesta[]
                {
                    new("You're right! It works now. Thank you so much.", "¡Tienes razón! Ya funciona. Muchas gracias."),
                    new("Okay, I'll try that and let you know.", "Bien, lo intento y te aviso.")
                })
        }),

        new("codereview", "Code review", "Explicar tu pull request, responder comentarios y aceptar cambios.", "lista", NivelMcer.B1, new Turno[]
        {
            new("Hey, I saw your pull request. What does it change?", "Oye, vi tu pull request. ¿Qué cambia?", new Respuesta[]
                {
                    new("It adds validation to the sign-up form.", "Agrega validación al formulario de registro."),
                    new("It fixes the bug in the payment service.", "Arregla el error del servicio de pagos."),
                    new("It refactors the user service to make it easier to test.", "Reorganiza el servicio de usuarios para que sea más fácil de probar.")
                }),
            new("Did you add tests for it?", "¿Le agregaste pruebas?", new Respuesta[]
                {
                    new("Yes, I added unit tests for the new logic.", "Sí, agregué pruebas unitarias para la lógica nueva."),
                    new("Not yet. I'll add them today.", "Todavía no. Las agrego hoy.")
                }),
            new("I left a comment about the variable names. Can you take a look?",
                "Dejé un comentario sobre los nombres de las variables. ¿Lo puedes revisar?", new Respuesta[]
                {
                    new("Sure, I'll take a look and update them.", "Claro, lo reviso y los cambio."),
                    new("Thanks for the feedback. I'll fix it right away.", "Gracias por los comentarios. Lo arreglo de inmediato.")
                }),
            new("Why did you use a dictionary here instead of a list?", "¿Por qué usaste un diccionario aquí en vez de una lista?", new Respuesta[]
                {
                    new("Because the lookup is faster with a dictionary.", "Porque buscar es más rápido con un diccionario."),
                    new("I used it to find users by their ID quickly.", "Lo usé para encontrar usuarios por su ID rápido."),
                    new("Good point. A list would work too. I can change it.", "Buen punto. Una lista también funcionaría. La puedo cambiar.")
                }),
            new("Looks good to me. I'll approve it.", "Se ve bien. Lo voy a aprobar.", new Respuesta[]
                {
                    new("Thanks for the review!", "¡Gracias por la revisión!"),
                    new("Thank you. I'll merge it after the tests pass.", "Gracias. Lo uno cuando pasen las pruebas.")
                })
        }),

        new("entrevista", "Entrevista: preséntate", "Tell me about yourself, tu experiencia y por qué quieres el puesto.", "maletin", NivelMcer.B1, new Turno[]
        {
            new("Hi, thanks for joining us today. Can you hear me well?",
                "Hola, gracias por acompañarnos hoy. ¿Me escuchas bien?", new Respuesta[]
                {
                    new("Yes, I can hear you well. Thanks for having me.", "Sí, te escucho bien. Gracias por recibirme."),
                    new("Yes, perfectly. Thank you for the opportunity.", "Sí, perfectamente. Gracias por la oportunidad.")
                }),
            new("Great. Tell me about yourself.", "Muy bien. Háblame de ti.", new Respuesta[]
                {
                    new("I'm a .NET developer from Mexico. I build web applications with C# and ASP.NET Core.",
                        "Soy desarrollador .NET de México. Hago aplicaciones web con C# y ASP.NET Core."),
                    new("I'm a software developer. I mostly work with C#, SQL, and ASP.NET Core.",
                        "Soy desarrollador de software. Trabajo sobre todo con C#, SQL y ASP.NET Core.")
                }),
            new("What kind of projects have you worked on?", "¿En qué tipo de proyectos has trabajado?", new Respuesta[]
                {
                    new("I've worked on internal tools and REST APIs.", "He trabajado en herramientas internas y APIs REST."),
                    new("I've built web applications with Entity Framework and SQL Server.", "He hecho aplicaciones web con Entity Framework y SQL Server."),
                    new("I built a web app to learn English with ASP.NET Core.", "Hice una aplicación web para aprender inglés con ASP.NET Core.")
                }),
            new("What is your strongest technical skill?", "¿Cuál es tu habilidad técnica más fuerte?", new Respuesta[]
                {
                    new("My strongest skill is backend development with C#.", "Mi habilidad más fuerte es el desarrollo backend con C#."),
                    new("I'm good at designing APIs and working with databases.", "Soy bueno diseñando APIs y trabajando con bases de datos.")
                }),
            new("Why do you want to work with us?", "¿Por qué quieres trabajar con nosotros?", new Respuesta[]
                {
                    new("I want to work on a team where I can learn and grow.", "Quiero trabajar en un equipo donde pueda aprender y crecer."),
                    new("I like your product, and I want to work with an international team.", "Me gusta su producto y quiero trabajar con un equipo internacional.")
                }),
            new("Do you have any questions for us?", "¿Tienes alguna pregunta para nosotros?", new Respuesta[]
                {
                    new("Yes. What does a typical day look like for this role?", "Sí. ¿Cómo es un día normal en este puesto?"),
                    new("How big is the team I would work with?", "¿Qué tan grande es el equipo con el que trabajaría?"),
                    new("What are the next steps in the process?", "¿Cuáles son los siguientes pasos del proceso?")
                })
        })
    };

    public static Situacion? Buscar(string? clave) => Todas.FirstOrDefault(s => s.Clave == clave);
}
