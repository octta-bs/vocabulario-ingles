using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Los temas de gramática que más usa un developer, enfocados en los errores típicos de quien habla español.
/// Cada tema trae su regla (en español, corta) y frases de trabajo con su versión con el error.
/// Están escritas a mano (inglés de Estados Unidos) y conviene revisarlas.
/// Las frases se guardan como palabras de tipo <see cref="TipoPalabra.Gramatica"/> en la categoría
/// "Gramática: {Titulo}" y solo entran al repaso cuando eliges practicar el tema.
/// </summary>
public static class TemasGramatica
{
    public const string PrefijoCategoria = "Gramática: ";

    /// <param name="Correcta">La frase bien escrita: es la respuesta del repaso.</param>
    /// <param name="Incorrecta">La misma frase con el error típico (ejercicio "Corrige la frase").</param>
    /// <param name="PalabraClave">Lo que se quita en el ejercicio de completar: la parte gramatical.</param>
    /// <param name="Por">Por qué: la regla aplicada a esta frase.</param>
    public record FraseGramatica(string Correcta, string Incorrecta, string Espanol, string PalabraClave, string Por);

    public record Tema(string Clave, string Titulo, NivelMcer Nivel, string Resumen, string[] Regla, FraseGramatica[] Frases)
    {
        public string Categoria => PrefijoCategoria + Titulo;
    }

    /// <summary>En orden de estudio: de lo más básico (A2) a lo más avanzado (B1).</summary>
    public static readonly Tema[] Todos =
    {
        new("tiempos-basicos", "Presente, pasado y futuro", NivelMcer.A2,
            "Los tres tiempos básicos: I fix, I fixed, I will fix.",
            new[]
            {
                "Presente simple para rutinas y hechos: I work, they work. Con he, she o it el verbo lleva -s: she works.",
                "En presente, la negación es don't o doesn't + el verbo sin cambios: He doesn't use Docker.",
                "Pasado simple para algo que ya terminó: los regulares llevan -ed (deployed); los irregulares cambian (write → wrote). La negación es didn't + el verbo sin cambios.",
                "Futuro: will + el verbo sin cambios: I will call you. Para planes ya decididos también se usa going to (tema 6)."
            },
            new FraseGramatica[]
            {
                new("She works from home on Fridays.", "She work from home on Fridays.",
                    "Ella trabaja desde casa los viernes.", "works",
                    "Con he, she o it, el presente lleva -s: works."),
                new("The build takes ten minutes.", "The build take ten minutes.",
                    "El build tarda diez minutos.", "takes",
                    "The build es \"it\": el presente lleva -s (takes)."),
                new("He doesn't use Docker.", "He don't use Docker.",
                    "Él no usa Docker.", "doesn't",
                    "Con he, she o it, la negación en presente es doesn't."),
                new("I deployed the app yesterday.", "I deploy the app yesterday.",
                    "Publiqué la aplicación ayer.", "deployed",
                    "Yesterday es pasado: deployed, con -ed."),
                new("We wrote the tests last week.", "We writed the tests last week.",
                    "Escribimos las pruebas la semana pasada.", "wrote",
                    "Write es irregular: su pasado es wrote, no \"writed\"."),
                new("I didn't finish the task.", "I didn't finished the task.",
                    "No terminé la tarea.", "finish",
                    "Con didn't, el verbo va sin cambios: finish, no finished."),
                new("I will call you tomorrow.", "I call you tomorrow.",
                    "Te llamo mañana.", "will",
                    "Para el futuro usa will + verbo: I will call. En español se dice en presente, en inglés no.")
            }),

        new("errores-tipicos", "Errores típicos de hispanohablantes", NivelMcer.A2,
            "Los que más delatan en una entrevista: \"I am agree\", \"people is\"…",
            new[]
            {
                "Agree ya es verbo: I agree (no \"I am agree\").",
                "Depend on, no \"depend of\". People es plural: people are.",
                "\"Tengo una duda\" es I have a question. Mistake va con make: make a mistake.",
                "Information no tiene plural. Excited es cómo te sientes; exciting, lo que causa la emoción."
            },
            new FraseGramatica[]
            {
                new("I agree with you.", "I am agree with you.",
                    "Estoy de acuerdo contigo.", "agree",
                    "Agree ya es verbo: I agree, sin am."),
                new("It depends on the client.", "It depends of the client.",
                    "Depende del cliente.", "on",
                    "Depend va con on: depends on."),
                new("People are very friendly here.", "People is very friendly here.",
                    "La gente aquí es muy amable.", "are",
                    "People es plural: people are."),
                new("I have a question about the API.", "I have a doubt about the API.",
                    "Tengo una duda sobre la API.", "question",
                    "\"Tengo una duda\" es I have a question; doubt suena a que desconfías."),
                new("I made a mistake in the query.", "I did a mistake in the query.",
                    "Cometí un error en la consulta.", "made",
                    "Mistake va con make: make a mistake (pasado: made)."),
                new("The information is on the wiki.", "The informations are on the wiki.",
                    "La información está en la wiki.", "information",
                    "Information no tiene plural: the information is."),
                new("I'm excited about the new project.", "I'm exciting about the new project.",
                    "Estoy emocionado por el nuevo proyecto.", "excited",
                    "Cómo te sientes lleva -ed (excited); lo que causa la emoción lleva -ing (exciting).")
            }),

        new("articulos", "A, an y the", NivelMcer.A2,
            "Cuándo poner artículo y cuál.",
            new[]
            {
                "Las profesiones llevan a/an: I am a developer (en español no se pone).",
                "An antes de sonido de vocal: an engineer, an hour (la h no suena). A antes de sonido de consonante: a useful tool.",
                "The para algo específico que los dos conocen: the link you mentioned.",
                "Al hablar en general no se usa the: Coffee helps me focus."
            },
            new FraseGramatica[]
            {
                new("I am a software developer.", "I am software developer.",
                    "Soy desarrollador de software.", "a",
                    "Las profesiones llevan a/an: I am a developer."),
                new("She is an engineer.", "She is a engineer.",
                    "Ella es ingeniera.", "an",
                    "Engineer empieza con sonido de vocal: an."),
                new("Coffee helps me focus.", "The coffee helps me focus.",
                    "El café me ayuda a concentrarme.", "Coffee",
                    "Al hablar en general (el café, cualquier café) no se usa the."),
                new("We need an hour to fix it.", "We need a hour to fix it.",
                    "Necesitamos una hora para arreglarlo.", "an",
                    "En hour la h no suena (\"aur\"): an hour."),
                new("Can you send me the link you mentioned?", "Can you send me link you mentioned?",
                    "¿Me mandas el link que mencionaste?", "the",
                    "Algo específico que los dos conocen lleva the."),
                new("It's a useful tool.", "It's an useful tool.",
                    "Es una herramienta útil.", "a",
                    "Useful suena con \"yu\" (consonante): a useful.")
            }),

        new("preguntas", "El orden de las preguntas", NivelMcer.A2,
            "Do, does y did, y dónde va cada palabra.",
            new[]
            {
                "En presente la pregunta lleva do/does antes del sujeto: What does this function do?",
                "En pasado lleva did y el verbo en forma base: Where did you find it? Did it work?",
                "Con is/are, el verbo va antes del sujeto: Why is the build failing?",
                "Una pregunta dentro de otra lleva el orden normal: Do you know where the logs are?"
            },
            new FraseGramatica[]
            {
                new("What does this function do?", "What this function does?",
                    "¿Qué hace esta función?", "does",
                    "En presente la pregunta lleva does antes del sujeto."),
                new("Where did you find the bug?", "Where you found the bug?",
                    "¿Dónde encontraste el bug?", "did",
                    "En pasado la pregunta lleva did y el verbo en forma base (find)."),
                new("Why is the build failing?", "Why the build is failing?",
                    "¿Por qué está fallando el build?", "is",
                    "Con is, el verbo va antes del sujeto en la pregunta."),
                new("How many users does the app have?", "How many users the app has?",
                    "¿Cuántos usuarios tiene la aplicación?", "does",
                    "En presente la pregunta lleva does, y el verbo queda en forma base (have)."),
                new("Do you know where the logs are?", "Do you know where are the logs?",
                    "¿Sabes dónde están los logs?", "the logs are",
                    "Una pregunta dentro de otra lleva el orden normal: where the logs are."),
                new("Did the deployment work?", "Did the deployment worked?",
                    "¿Funcionó el despliegue?", "work",
                    "Con did, el verbo va en forma base: work, no worked.")
            }),

        new("preposiciones", "In, on y at", NivelMcer.A2,
            "Las preposiciones de tiempo y lugar que más se confunden.",
            new[]
            {
                "At para horas: at nine o'clock, at noon.",
                "On para días y fechas: on Monday, on May 5, on the weekend (así se dice en EE. UU.).",
                "In para meses, años y estaciones: in March, in 2024.",
                "Lugar: in = dentro (in the folder); on = sobre una superficie o pantalla (on the screen)."
            },
            new FraseGramatica[]
            {
                new("The meeting is on Monday.", "The meeting is in Monday.",
                    "La reunión es el lunes.", "on",
                    "Días y fechas: on."),
                new("I start work at nine o'clock.", "I start work in nine o'clock.",
                    "Empiezo a trabajar a las nueve.", "at",
                    "Horas: at."),
                new("We released it in March.", "We released it on March.",
                    "Lo publicamos en marzo.", "in",
                    "Meses, años y estaciones: in."),
                new("The file is in the shared folder.", "The file is on the shared folder.",
                    "El archivo está en la carpeta compartida.", "in",
                    "Dentro de una carpeta: in."),
                new("I saw the error on the screen.", "I saw the error in the screen.",
                    "Vi el error en la pantalla.", "on",
                    "Sobre una pantalla: on the screen."),
                new("We have a deployment on the weekend.", "We have a deployment in the weekend.",
                    "Tenemos un despliegue el fin de semana.", "on",
                    "En inglés de EE. UU. se dice on the weekend.")
            }),

        new("futuro", "Will o going to", NivelMcer.A2,
            "Planes, decisiones del momento y el famoso \"I go to\".",
            new[]
            {
                "Going to para planes ya decididos: I'm going to deploy it this afternoon.",
                "Will para decisiones en el momento, promesas y predicciones: I'll send you the link.",
                "\"Voy a…\" no es \"I go to…\"; es I'm going to + verbo.",
                "Después de will o going to va el verbo en forma base, sin to y sin -ing: will be, going to migrate."
            },
            new FraseGramatica[]
            {
                new("I'm going to deploy the fix this afternoon.", "I go to deploy the fix this afternoon.",
                    "Voy a publicar el arreglo esta tarde.", "going",
                    "\"Voy a\" (un plan) es I'm going to, no \"I go to\"."),
                new("I'll send you the link right now.", "I send you the link right now.",
                    "Ahora mismo te mando el link.", "I'll",
                    "Para algo que decides en el momento (\"ahora te lo mando\") usa will."),
                new("It will be ready tomorrow.", "It will to be ready tomorrow.",
                    "Estará listo mañana.", "be",
                    "Después de will va el verbo sin to: will be."),
                new("We are going to migrate the database next week.", "We are going to migrating the database next week.",
                    "Vamos a migrar la base de datos la próxima semana.", "migrate",
                    "Going to + verbo en forma base: migrate, no migrating."),
                new("Will you join the meeting?", "You will join the meeting?",
                    "¿Vas a entrar a la reunión?", "Will",
                    "En una pregunta, will va antes del sujeto: Will you…?"),
                new("Are you going to work on the bug today?", "You are going to work on the bug today?",
                    "¿Vas a trabajar en el bug hoy?", "Are",
                    "En una pregunta, are va antes del sujeto: Are you going to…?")
            }),

        new("cortesia", "Pedir con cortesía", NivelMcer.A2,
            "Could you…?, Would you mind…? y I would like to…",
            new[]
            {
                "Para pedir algo con cortesía: Could you…? Suena mejor que \"You can…?\".",
                "Would you mind + verbo con -ing: Would you mind sharing your screen?",
                "I would like to… es más amable que I want.",
                "Explicar algo A alguien: explain it to me (no \"explain me\")."
            },
            new FraseGramatica[]
            {
                new("Could you review my pull request, please?", "You can review my pull request, please?",
                    "¿Podrías revisar mi pull request, por favor?", "Could",
                    "Para pedir algo con cortesía: Could you…?"),
                new("Would you mind sharing your screen?", "Would you mind to share your screen?",
                    "¿Te molestaría compartir tu pantalla?", "sharing",
                    "Después de would you mind va el verbo con -ing: sharing."),
                new("I would like to ask a question.", "I want ask a question.",
                    "Me gustaría hacer una pregunta.", "would like",
                    "I would like to… es más cortés que I want, y lleva to."),
                new("Could you explain it to me again?", "Could you explain me it again?",
                    "¿Me lo podrías explicar otra vez?", "to me",
                    "Se explica algo A alguien: explain it to me, no \"explain me\"."),
                new("Do you have a minute to talk?", "Do you have a minute for talk?",
                    "¿Tienes un minuto para hablar?", "to talk",
                    "\"Para\" + verbo es to + verbo: to talk, no \"for talk\"."),
                new("May I ask why you chose this approach?", "May I ask why did you choose this approach?",
                    "¿Puedo preguntar por qué elegiste esta solución?", "you chose",
                    "Una pregunta dentro de otra lleva el orden normal: why you chose, sin did.")
            }),

        new("presente-perfecto", "Present perfect o pasado", NivelMcer.B1,
            "Cuándo decir \"I have worked\" y cuándo \"I worked\".",
            new[]
            {
                "Present perfect (have/has + participio) para algo que empezó en el pasado y sigue, o sin decir cuándo pasó.",
                "Pasado simple para algo terminado en un momento concreto: yesterday, last week, in 2021, from 2019 to 2021.",
                "Después de have/has va siempre el participio: worked, written, approved.",
                "En español dices \"trabajo con C# desde hace tres años\" (presente); en inglés va present perfect."
            },
            new FraseGramatica[]
            {
                new("I have worked with C# for three years.", "I work with C# for three years.",
                    "Llevo tres años trabajando con C#.", "have worked",
                    "Algo que empezó en el pasado y sigue: present perfect (have worked), no presente."),
                new("I fixed the bug yesterday.", "I have fixed the bug yesterday.",
                    "Arreglé el bug ayer.", "fixed",
                    "Con un momento terminado (yesterday) va el pasado simple, no el present perfect."),
                new("I have never worked with Python.", "I have never work with Python.",
                    "Nunca he trabajado con Python.", "worked",
                    "Después de have va el participio: worked, no work."),
                new("We released the new version last Friday.", "We have released the new version last Friday.",
                    "Publicamos la nueva versión el viernes pasado.", "released",
                    "Last Friday es un momento terminado: pasado simple."),
                new("Has the client approved the design?", "Has the client approve the design?",
                    "¿El cliente ya aprobó el diseño?", "approved",
                    "Has + participio: approved."),
                new("I worked at a bank from 2019 to 2021.", "I have worked at a bank from 2019 to 2021.",
                    "Trabajé en un banco de 2019 a 2021.", "worked",
                    "Si ya terminó (de 2019 a 2021), va el pasado simple.")
            }),

        new("for-since", "For y since", NivelMcer.B1,
            "\"Llevo dos años\" y \"desde 2020\" sin traducir palabra por palabra.",
            new[]
            {
                "For + cantidad de tiempo: for two years, for an hour, for a long time.",
                "Since + el momento en que empezó: since 2020, since Monday, since ten o'clock.",
                "\"Llevo…\" o \"estoy … desde\" se dice con present perfect: I have been working, I have lived.",
                "\"¿Cuánto llevas…?\" = How long have you been + -ing?"
            },
            new FraseGramatica[]
            {
                new("I have been working here for two years.", "I have been working here since two years.",
                    "Llevo dos años trabajando aquí.", "for",
                    "Two years es una cantidad de tiempo: for, no since."),
                new("I have lived in Mexico City since 2020.", "I have lived in Mexico City for 2020.",
                    "Vivo en la Ciudad de México desde 2020.", "since",
                    "2020 es el momento en que empezó: since."),
                new("I have been waiting for the build since ten o'clock.", "I am waiting for the build since ten o'clock.",
                    "Estoy esperando el build desde las diez.", "have been waiting",
                    "\"Estoy esperando desde\" en inglés es present perfect continuous: have been waiting."),
                new("How long have you been using React?", "How long are you using React?",
                    "¿Cuánto tiempo llevas usando React?", "have you been using",
                    "\"¿Cuánto llevas…?\" = How long have you been + -ing."),
                new("We have known each other for a long time.", "We have known each other since a long time.",
                    "Nos conocemos desde hace mucho.", "for",
                    "A long time es una cantidad de tiempo: for."),
                new("The server has been down for an hour.", "The server has been down since an hour.",
                    "El servidor lleva una hora caído.", "for",
                    "An hour es una cantidad de tiempo: for.")
            }),

        new("pasiva", "Voz pasiva", NivelMcer.B1,
            "\"Se publicó\", \"fue arreglado\": was/is + participio.",
            new[]
            {
                "La pasiva es is/are/was/were (o has been) + participio: The app was deployed.",
                "Sirve para decir qué pasó sin decir quién lo hizo, y se usa mucho en el trabajo.",
                "Ojo con los participios irregulares: run → run, write → written.",
                "Para decir quién lo hizo, usa by: requested by the client."
            },
            new FraseGramatica[]
            {
                new("The app was deployed yesterday.", "The app was deploy yesterday.",
                    "La aplicación se publicó ayer.", "deployed",
                    "Pasiva: was + participio (deployed)."),
                new("The tests are run every night.", "The tests are runned every night.",
                    "Las pruebas se corren cada noche.", "run",
                    "Run es irregular: su participio es run."),
                new("This feature was requested by the client.", "This feature was requested for the client.",
                    "Esta función la pidió el cliente.", "by",
                    "Para decir quién lo hizo en pasiva: by."),
                new("The bug has been fixed.", "The bug has been fix.",
                    "El bug ya se arregló.", "fixed",
                    "Has been + participio: fixed."),
                new("The documentation was written in English.", "The documentation was wrote in English.",
                    "La documentación se escribió en inglés.", "written",
                    "Write: wrote es el pasado, written el participio. La pasiva usa el participio."),
                new("Our code is reviewed by two people.", "Our code is review by two people.",
                    "Nuestro código lo revisan dos personas.", "reviewed",
                    "Pasiva: is + participio (reviewed).")
            }),

        new("condicional", "Primer condicional (if)", NivelMcer.B1,
            "If + presente, y will en la otra parte.",
            new[]
            {
                "Para algo posible en el futuro: If + presente, will + verbo.",
                "Después de if no va will, aunque hables del futuro: If the tests pass…",
                "Lo mismo con when y let me know if: Let me know if you need anything.",
                "La parte con if puede ir al principio o al final: We will miss the deadline if we don't hurry."
            },
            new FraseGramatica[]
            {
                new("If the tests pass, I will merge the branch.", "If the tests will pass, I will merge the branch.",
                    "Si pasan las pruebas, uniré la rama.", "pass",
                    "Después de if va presente, no will."),
                new("If you have time, could you check my code?", "If you will have time, could you check my code?",
                    "Si tienes tiempo, ¿podrías revisar mi código?", "have",
                    "Después de if va presente: if you have time."),
                new("If it doesn't work, I'll roll back the changes.", "If it won't work, I'll roll back the changes.",
                    "Si no funciona, revierto los cambios.", "doesn't",
                    "Después de if va presente: doesn't, no won't."),
                new("We will miss the deadline if we don't hurry.", "We will miss the deadline if we won't hurry.",
                    "No llegaremos a la fecha límite si no nos apuramos.", "don't",
                    "Aunque if vaya al final, después de if va presente: don't."),
                new("If I finish early, I will help you.", "If I will finish early, I will help you.",
                    "Si termino temprano, te ayudo.", "finish",
                    "Después de if va presente: if I finish."),
                new("Let me know if you need anything.", "Let me know if you will need anything.",
                    "Avísame si necesitas algo.", "need",
                    "Después de if va presente: if you need.")
            })
    };

    public static Tema? Buscar(string? clave) => Todos.FirstOrDefault(t => t.Clave == clave);

    public static bool EsCategoriaDeGramatica(string nombre) => nombre.StartsWith(PrefijoCategoria, StringComparison.Ordinal);
}
