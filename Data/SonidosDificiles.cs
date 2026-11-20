namespace VocabularioIngles.Data;

/// <summary>
/// Los sonidos del inglés de EE. UU. que más cuestan a quien habla español, en orden de estudio.
/// Cada uno se practica con pares mínimos: dos palabras que solo cambian en ese sonido (ship / sheep).
/// Las explicaciones están escritas a mano; los pares son palabras comunes. No hay datos de pronunciación por
/// palabra: el audio sale de la voz del navegador (en-US).
/// </summary>
public static class SonidosDificiles
{
    /// <param name="Con">La palabra con el sonido que se practica (ship).</param>
    /// <param name="Contra">La palabra con la que se confunde (sheep).</param>
    public record Par(string Con, string Contra);

    /// <param name="Simbolos">Los sonidos en símbolos fonéticos, para quien los quiera ver (/ɪ/ y /i/).</param>
    /// <param name="ComoSeHace">Cómo poner la boca, en español y en pocas frases.</param>
    public record Sonido(string Clave, string Titulo, string Simbolos, string Resumen, string[] ComoSeHace, Par[] Pares);

    /// <summary>En orden de estudio: primero los que se corrigen más fácil.</summary>
    public static readonly Sonido[] Todos =
    {
        new("e-inicial", "Sin \"e\" antes de s", "/st/ y /ɪst/",
            "\"Estudent\" en vez de \"student\": la e que el español agrega.",
            new[]
            {
                "En español ninguna palabra empieza con s + consonante, así que agregamos una e: \"estado\".",
                "En inglés la palabra empieza directo con la s: alarga la sssss y entra a la siguiente letra.",
                "Truco: piensa que la palabra anterior termina en s (\"this state\") y únelas."
            },
            new Par[] { new("state", "estate"), new("steam", "esteem"), new("special", "especial") }),

        new("v-b", "V y B", "/v/ y /b/",
            "Very no es berry: en inglés la v suena distinto.",
            new[]
            {
                "En español v y b suenan igual; en inglés no.",
                "B: junta los dos labios, como en español.",
                "V: apoya los dientes de arriba en el labio de abajo y deja salir el aire con vibración (como un zumbido)."
            },
            new Par[] { new("very", "berry"), new("vest", "best"), new("vote", "boat"), new("van", "ban") }),

        new("h", "La h que sí suena", "/h/",
            "Hat no es at: la h inglesa se oye.",
            new[]
            {
                "En español la h no suena; en inglés casi siempre sí.",
                "Es un soplo suave, como cuando empañas un vidrio: más suave que nuestra j.",
                "Sin ese soplo, hat se oye at y hear se oye ear."
            },
            new Par[] { new("hat", "at"), new("heat", "eat"), new("hold", "old"), new("hear", "ear") }),

        new("i-corta", "I corta e i larga", "/ɪ/ y /i/",
            "Ship no es sheep: la diferencia más famosa.",
            new[]
            {
                "La i larga (sheep, leave) es como nuestra i, un poco más larga y con los labios estirados.",
                "La i corta (ship, live) es más relajada y corta: está entre una i y una e.",
                "Si dices todas como la i del español, ship se oye sheep."
            },
            new Par[] { new("ship", "sheep"), new("live", "leave"), new("fill", "feel"), new("sit", "seat"), new("bit", "beat") }),

        new("sh-ch", "Sh y ch", "/ʃ/ y /tʃ/",
            "Share no es chair: la sh es un sonido suave y continuo.",
            new[]
            {
                "Ch es como en español (chocolate): un golpe corto.",
                "Sh es el sonido de pedir silencio (shhh): sin golpe, el aire sale seguido.",
                "Si dices sh como ch, share se oye chair y wash se oye watch."
            },
            new Par[] { new("share", "chair"), new("ship", "chip"), new("wash", "watch"), new("shoes", "choose") }),

        new("z", "La z que zumba", "/z/ y /s/",
            "Prize no es price: la z inglesa vibra.",
            new[]
            {
                "En el español de México la z suena como s; en inglés la z es una s con vibración.",
                "Pon la lengua como para la s y haz vibrar la garganta, como una abeja: zzzz.",
                "Muchas s al final suenan z: eyes, peas, buzz."
            },
            new Par[] { new("prize", "price"), new("eyes", "ice"), new("buzz", "bus"), new("peas", "peace") }),

        new("th-sorda", "Th sorda", "/θ/",
            "Think no es sink ni tink.",
            new[]
            {
                "Pon la punta de la lengua entre los dientes, un poquito afuera.",
                "Sopla sin hacer vibrar la garganta: es como la z de España.",
                "Si usas s, think se oye sink; si usas t, three se oye tree."
            },
            new Par[] { new("think", "sink"), new("three", "tree"), new("thank", "tank"), new("path", "pass"), new("thick", "sick") }),

        new("th-sonora", "Th sonora", "/ð/",
            "They no es day: la th de the, this y that.",
            new[]
            {
                "La lengua va igual que en la th sorda: entre los dientes.",
                "Pero ahora la garganta vibra, como la d suave de \"nada\".",
                "Si dices una d fuerte, they se oye day y then se oye den."
            },
            new Par[] { new("they", "day"), new("then", "den"), new("though", "dough"), new("breathe", "breed") }),

        new("a-abierta", "A abierta y a corta", "/æ/ y /ʌ/",
            "Cat no es cut: dos vocales que el español no tiene.",
            new[]
            {
                "La a de cat es muy abierta: abre la boca como para decir a, pero estira los labios como para decir e.",
                "La u de cut es una a corta y relajada, con la boca casi cerrada: como un \"ah\" de sorpresa rápido.",
                "Si dices las dos como nuestra a, cat y cut suenan igual."
            },
            new Par[] { new("cat", "cut"), new("bag", "bug"), new("cap", "cup"), new("ran", "run") }),

        new("ed", "La terminación -ed", "/t/, /d/ y /ɪd/",
            "Worked no se dice \"work-ed\": la -ed casi nunca es una sílaba.",
            new[]
            {
                "Después de sonidos sordos (k, p, s, sh, ch, f) suena t: worked = \"workt\".",
                "Después de sonidos sonoros (vocales, l, n, m, g, v) suena d: played = \"playd\".",
                "Solo después de t o d se agrega una sílaba: wanted = \"want-id\", needed = \"need-id\".",
                "Si no se oye la -ed, worked se oye work y la frase queda en presente."
            },
            new Par[] { new("worked", "work"), new("fixed", "fix"), new("wanted", "want"), new("needed", "need") })
    };

    public static Sonido? Buscar(string? clave) => Todos.FirstOrDefault(s => s.Clave == clave);
}
