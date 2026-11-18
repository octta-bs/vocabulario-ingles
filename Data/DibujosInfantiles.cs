using System.Globalization;
using System.Text;

namespace VocabularioIngles.Data;

/// <summary>
/// Dibujos sencillos (SVG hechos a mano, sin licencias de terceros) para el modo infantil.
/// Colores: un círculo de ese color. Números: el dígito grande y puntitos para contar.
/// Animales: caritas. Cuerpo: una figura con la parte que se pregunta resaltada en rojo.
/// La clave es la palabra en inglés en minúsculas.
/// </summary>
public static class DibujosInfantiles
{
    // Categorías del modo infantil (deben existir en VocabularioGeneral).
    public static readonly string[] Categorias = { "Colores", "Números", "Animales", "El cuerpo" };

    private const string Piel = "#F2C6A0";
    private const string Rasgos = "#3B2F2F";
    private const string Resalte = "#E53935";

    private static readonly Dictionary<string, string> ColoresHex = new()
    {
        ["red"] = "#E53935", ["blue"] = "#1E88E5", ["green"] = "#43A047", ["yellow"] = "#FDD835",
        ["black"] = "#212121", ["white"] = "#FFFFFF", ["orange"] = "#FB8C00", ["pink"] = "#EC407A",
        ["purple"] = "#8E24AA", ["brown"] = "#6D4C41", ["gray"] = "#9E9E9E"
    };

    private static readonly Dictionary<string, int> Numeros = new()
    {
        ["one"] = 1, ["two"] = 2, ["three"] = 3, ["four"] = 4, ["five"] = 5, ["six"] = 6, ["seven"] = 7,
        ["eight"] = 8, ["nine"] = 9, ["ten"] = 10, ["twenty"] = 20, ["hundred"] = 100
    };

    public static bool Existe(string ingles) => Contenido(ingles.ToLowerInvariant()) != null;

    /// <summary>El SVG completo del dibujo, o null si la palabra no tiene dibujo.</summary>
    public static string? Svg(string ingles)
    {
        var contenido = Contenido(ingles.ToLowerInvariant());
        return contenido == null
            ? null
            : $"<svg viewBox=\"0 0 100 100\" xmlns=\"http://www.w3.org/2000/svg\" aria-hidden=\"true\" focusable=\"false\">{contenido}</svg>";
    }

    private static string? Contenido(string clave)
    {
        if (ColoresHex.TryGetValue(clave, out var hex))
        {
            var borde = clave == "white" ? " stroke=\"#9E9E9E\" stroke-width=\"3\"" : "";
            return $"<circle cx=\"50\" cy=\"52\" r=\"38\" fill=\"#000\" opacity=\"0.08\"/><circle cx=\"50\" cy=\"50\" r=\"38\" fill=\"{hex}\"{borde}/>" +
                   "<ellipse cx=\"38\" cy=\"36\" rx=\"10\" ry=\"6\" fill=\"#fff\" opacity=\"0.35\" transform=\"rotate(-30 38 36)\"/>";
        }

        if (Numeros.TryGetValue(clave, out var numero))
        {
            return NumeroConPuntos(numero);
        }

        return clave switch
        {
            "color" => Paleta(),
            "dog" => Perro,
            "cat" => Gato,
            "bird" => Pajaro,
            "fish" => Pez,
            "horse" => Caballo,
            "cow" => Vaca,
            "pig" => Cerdo,
            "duck" => Pato,
            "rabbit" => Conejo,
            "lion" => Leon,
            "elephant" => Elefante,
            "monkey" => Mono,
            "head" => Figura("head"),
            "hand" => Figura("hand"),
            "arm" => Figura("arm"),
            "leg" => Figura("leg"),
            "foot" => Figura("foot"),
            "back" => FiguraDeEspaldas(),
            "eye" => Cara("eye"),
            "ear" => Cara("ear"),
            "nose" => Cara("nose"),
            "mouth" => Cara("mouth"),
            "hair" => Cara("hair"),
            "face" => Cara("face"),
            _ => null
        };
    }

    private static string NumeroConPuntos(int numero)
    {
        var texto = numero.ToString(CultureInfo.InvariantCulture);
        var tamano = texto.Length switch { 1 => 54, 2 => 46, _ => 36 };
        var sb = new StringBuilder();
        sb.Append("<rect x=\"8\" y=\"8\" width=\"84\" height=\"84\" rx=\"18\" fill=\"#FFF3E0\"/>");
        var y = numero <= 10 ? 52 : 64;
        sb.Append($"<text x=\"50\" y=\"{y}\" text-anchor=\"middle\" font-family=\"system-ui, sans-serif\" font-weight=\"800\" font-size=\"{tamano}\" fill=\"#E65100\">{texto}</text>");

        // Puntitos para contar (del 1 al 10), en filas de 5.
        if (numero <= 10)
        {
            for (var i = 0; i < numero; i++)
            {
                var fila = i / 5;
                var enFila = Math.Min(5, numero - fila * 5);
                var x = 50 - (enFila - 1) * 7 + (i % 5) * 14;
                sb.Append($"<circle cx=\"{x}\" cy=\"{68 + fila * 13}\" r=\"5\" fill=\"#1E88E5\"/>");
            }
        }

        return sb.ToString();
    }

    private static string Paleta()
    {
        var colores = new[] { "#E53935", "#FB8C00", "#FDD835", "#43A047", "#1E88E5", "#8E24AA" };
        var sb = new StringBuilder("<path d=\"M50 12 C78 12 92 32 90 52 C88 70 74 70 66 66 C58 62 52 70 56 80 C60 90 40 92 28 84 C12 74 8 54 14 38 C20 22 34 12 50 12 Z\" fill=\"#F5DEB3\" stroke=\"#C8A97E\" stroke-width=\"2\"/>");
        var puntos = new[] { (30, 38), (46, 26), (64, 28), (76, 44), (28, 58), (38, 74) };
        for (var i = 0; i < colores.Length; i++)
        {
            sb.Append($"<circle cx=\"{puntos[i].Item1}\" cy=\"{puntos[i].Item2}\" r=\"8\" fill=\"{colores[i]}\"/>");
        }

        return sb.ToString();
    }

    // ---------- Animales (caritas) ----------

    private const string Ojos = "<circle cx=\"40\" cy=\"48\" r=\"4.5\" fill=\"" + Rasgos + "\"/><circle cx=\"60\" cy=\"48\" r=\"4.5\" fill=\"" + Rasgos + "\"/>" +
                                "<circle cx=\"41.5\" cy=\"46.5\" r=\"1.5\" fill=\"#fff\"/><circle cx=\"61.5\" cy=\"46.5\" r=\"1.5\" fill=\"#fff\"/>";

    private const string Perro =
        "<ellipse cx=\"22\" cy=\"52\" rx=\"11\" ry=\"22\" fill=\"#6D4C41\" transform=\"rotate(15 22 52)\"/>" +
        "<ellipse cx=\"78\" cy=\"52\" rx=\"11\" ry=\"22\" fill=\"#6D4C41\" transform=\"rotate(-15 78 52)\"/>" +
        "<circle cx=\"50\" cy=\"52\" r=\"30\" fill=\"#C68A55\"/>" +
        "<ellipse cx=\"50\" cy=\"66\" rx=\"16\" ry=\"12\" fill=\"#F3D9B8\"/>" + Ojos +
        "<ellipse cx=\"50\" cy=\"60\" rx=\"6\" ry=\"4.5\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M44 70 Q50 75 56 70\" stroke=\"" + Rasgos + "\" stroke-width=\"2\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "<ellipse cx=\"53\" cy=\"76\" rx=\"4\" ry=\"5\" fill=\"#EF5350\"/>";

    private const string Gato =
        "<path d=\"M24 40 L28 12 L46 30 Z\" fill=\"#F4A340\"/><path d=\"M76 40 L72 12 L54 30 Z\" fill=\"#F4A340\"/>" +
        "<path d=\"M28 34 L30 20 L40 30 Z\" fill=\"#F8BBD0\"/><path d=\"M72 34 L70 20 L60 30 Z\" fill=\"#F8BBD0\"/>" +
        "<circle cx=\"50\" cy=\"54\" r=\"30\" fill=\"#F4A340\"/>" +
        "<path d=\"M38 30 L40 38 M50 26 L50 36 M62 30 L60 38\" stroke=\"#D9822B\" stroke-width=\"3\" stroke-linecap=\"round\"/>" +
        "<ellipse cx=\"40\" cy=\"50\" rx=\"4\" ry=\"6\" fill=\"" + Rasgos + "\"/><ellipse cx=\"60\" cy=\"50\" rx=\"4\" ry=\"6\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M46 60 L54 60 L50 65 Z\" fill=\"#EC407A\"/>" +
        "<path d=\"M50 65 Q46 70 42 68 M50 65 Q54 70 58 68\" stroke=\"" + Rasgos + "\" stroke-width=\"2\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "<path d=\"M18 58 L36 60 M18 66 L36 64 M82 58 L64 60 M82 66 L64 64\" stroke=\"" + Rasgos + "\" stroke-width=\"1.5\" stroke-linecap=\"round\"/>";

    private const string Pajaro =
        "<ellipse cx=\"48\" cy=\"60\" rx=\"28\" ry=\"22\" fill=\"#42A5F5\"/>" +
        "<circle cx=\"66\" cy=\"38\" r=\"16\" fill=\"#42A5F5\"/>" +
        "<path d=\"M80 36 L94 41 L80 45 Z\" fill=\"#FFA726\"/>" +
        "<circle cx=\"70\" cy=\"34\" r=\"3.5\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M30 56 Q46 46 56 62 Q42 70 30 56 Z\" fill=\"#1E88E5\"/>" +
        "<path d=\"M22 58 L8 50 L12 64 Z\" fill=\"#1E88E5\"/>" +
        "<path d=\"M42 80 L40 92 M54 80 L56 92\" stroke=\"#FFA726\" stroke-width=\"3\" stroke-linecap=\"round\"/>";

    private const string Pez =
        "<path d=\"M74 50 L94 32 L94 68 Z\" fill=\"#FB8C00\"/>" +
        "<ellipse cx=\"46\" cy=\"50\" rx=\"34\" ry=\"22\" fill=\"#FFA726\"/>" +
        "<path d=\"M40 30 Q50 18 62 32\" fill=\"#FB8C00\"/>" +
        "<path d=\"M52 42 Q58 50 52 58 M62 42 Q68 50 62 58\" stroke=\"#FB8C00\" stroke-width=\"3\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "<circle cx=\"28\" cy=\"46\" r=\"6\" fill=\"#fff\"/><circle cx=\"27\" cy=\"46\" r=\"3.5\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M14 56 Q18 58 22 56\" stroke=\"" + Rasgos + "\" stroke-width=\"2\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "<circle cx=\"10\" cy=\"34\" r=\"3\" fill=\"none\" stroke=\"#90CAF9\" stroke-width=\"2\"/><circle cx=\"6\" cy=\"24\" r=\"2\" fill=\"none\" stroke=\"#90CAF9\" stroke-width=\"2\"/>";

    private const string Caballo =
        "<path d=\"M30 92 L34 46 Q36 20 58 14 L64 6 L68 18 Q86 30 88 58 Q90 72 78 74 Q66 74 62 62 L56 92 Z\" fill=\"#A1683A\"/>" +
        "<path d=\"M58 14 Q40 18 34 46 L30 92 L22 92 L26 44 Q30 14 58 12 Z\" fill=\"#4E342E\"/>" +
        "<ellipse cx=\"78\" cy=\"64\" rx=\"11\" ry=\"9\" fill=\"#C8956A\"/>" +
        "<circle cx=\"80\" cy=\"62\" r=\"2\" fill=\"" + Rasgos + "\"/>" +
        "<circle cx=\"66\" cy=\"34\" r=\"4\" fill=\"" + Rasgos + "\"/><circle cx=\"67.5\" cy=\"32.5\" r=\"1.3\" fill=\"#fff\"/>";

    private const string Vaca =
        "<path d=\"M28 28 Q18 18 22 10 M72 28 Q82 18 78 10\" stroke=\"#E0D6C2\" stroke-width=\"6\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "<ellipse cx=\"18\" cy=\"40\" rx=\"12\" ry=\"7\" fill=\"#F5F5F5\" stroke=\"#BDBDBD\" stroke-width=\"1.5\"/>" +
        "<ellipse cx=\"82\" cy=\"40\" rx=\"12\" ry=\"7\" fill=\"#F5F5F5\" stroke=\"#BDBDBD\" stroke-width=\"1.5\"/>" +
        "<ellipse cx=\"50\" cy=\"50\" rx=\"28\" ry=\"32\" fill=\"#FAFAFA\" stroke=\"#BDBDBD\" stroke-width=\"1.5\"/>" +
        "<path d=\"M26 36 Q34 26 44 34 Q40 46 28 44 Z\" fill=\"#212121\"/><path d=\"M66 28 Q76 30 74 42 Q64 40 62 32 Z\" fill=\"#212121\"/>" +
        Ojos +
        "<ellipse cx=\"50\" cy=\"70\" rx=\"20\" ry=\"13\" fill=\"#F8BBD0\"/>" +
        "<ellipse cx=\"43\" cy=\"70\" rx=\"3\" ry=\"4\" fill=\"#C2185B\"/><ellipse cx=\"57\" cy=\"70\" rx=\"3\" ry=\"4\" fill=\"#C2185B\"/>";

    private const string Cerdo =
        "<path d=\"M22 34 L26 14 L42 26 Z\" fill=\"#F48FB1\"/><path d=\"M78 34 L74 14 L58 26 Z\" fill=\"#F48FB1\"/>" +
        "<circle cx=\"50\" cy=\"54\" r=\"32\" fill=\"#F8BBD0\"/>" + Ojos +
        "<ellipse cx=\"50\" cy=\"64\" rx=\"14\" ry=\"10\" fill=\"#F48FB1\"/>" +
        "<ellipse cx=\"45\" cy=\"64\" rx=\"3\" ry=\"4\" fill=\"#AD1457\"/><ellipse cx=\"55\" cy=\"64\" rx=\"3\" ry=\"4\" fill=\"#AD1457\"/>" +
        "<path d=\"M42 78 Q50 82 58 78\" stroke=\"#AD1457\" stroke-width=\"2\" fill=\"none\" stroke-linecap=\"round\"/>";

    private const string Pato =
        "<path d=\"M6 84 Q50 74 94 84 L94 96 L6 96 Z\" fill=\"#81D4FA\"/>" +
        "<ellipse cx=\"46\" cy=\"68\" rx=\"32\" ry=\"18\" fill=\"#FFEE58\"/>" +
        "<path d=\"M18 62 L8 52 L22 58 Z\" fill=\"#FDD835\"/>" +
        "<circle cx=\"68\" cy=\"40\" r=\"16\" fill=\"#FFEE58\"/>" +
        "<path d=\"M80 40 Q96 40 94 46 Q86 50 80 46 Z\" fill=\"#FB8C00\"/>" +
        "<circle cx=\"70\" cy=\"36\" r=\"3.5\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M32 64 Q46 56 58 66 Q46 76 32 64 Z\" fill=\"#FDD835\"/>";

    private const string Conejo =
        "<ellipse cx=\"36\" cy=\"24\" rx=\"9\" ry=\"22\" fill=\"#E0E0E0\"/><ellipse cx=\"64\" cy=\"24\" rx=\"9\" ry=\"22\" fill=\"#E0E0E0\"/>" +
        "<ellipse cx=\"36\" cy=\"24\" rx=\"4.5\" ry=\"16\" fill=\"#F8BBD0\"/><ellipse cx=\"64\" cy=\"24\" rx=\"4.5\" ry=\"16\" fill=\"#F8BBD0\"/>" +
        "<circle cx=\"50\" cy=\"60\" r=\"28\" fill=\"#EEEEEE\"/>" +
        "<circle cx=\"40\" cy=\"56\" r=\"4.5\" fill=\"" + Rasgos + "\"/><circle cx=\"60\" cy=\"56\" r=\"4.5\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M46 64 L54 64 L50 68 Z\" fill=\"#EC407A\"/>" +
        "<rect x=\"46\" y=\"72\" width=\"8\" height=\"8\" rx=\"1.5\" fill=\"#fff\" stroke=\"#BDBDBD\"/>" +
        "<path d=\"M50 68 L50 72\" stroke=\"" + Rasgos + "\" stroke-width=\"1.5\"/>";

    private const string Leon =
        "<circle cx=\"50\" cy=\"52\" r=\"42\" fill=\"#E65100\"/>" +
        "<circle cx=\"28\" cy=\"28\" r=\"8\" fill=\"#FFB74D\"/><circle cx=\"72\" cy=\"28\" r=\"8\" fill=\"#FFB74D\"/>" +
        "<circle cx=\"50\" cy=\"54\" r=\"28\" fill=\"#FFB74D\"/>" + Ojos +
        "<ellipse cx=\"50\" cy=\"66\" rx=\"12\" ry=\"9\" fill=\"#FFE0B2\"/>" +
        "<path d=\"M45 61 L55 61 L50 66 Z\" fill=\"#5D4037\"/>" +
        "<path d=\"M50 66 Q46 72 42 70 M50 66 Q54 72 58 70\" stroke=\"" + Rasgos + "\" stroke-width=\"2\" fill=\"none\" stroke-linecap=\"round\"/>";

    private const string Elefante =
        "<ellipse cx=\"20\" cy=\"46\" rx=\"18\" ry=\"24\" fill=\"#90A4AE\"/><ellipse cx=\"80\" cy=\"46\" rx=\"18\" ry=\"24\" fill=\"#90A4AE\"/>" +
        "<ellipse cx=\"20\" cy=\"46\" rx=\"10\" ry=\"16\" fill=\"#F8BBD0\" opacity=\"0.6\"/><ellipse cx=\"80\" cy=\"46\" rx=\"10\" ry=\"16\" fill=\"#F8BBD0\" opacity=\"0.6\"/>" +
        "<circle cx=\"50\" cy=\"44\" r=\"26\" fill=\"#B0BEC5\"/>" +
        "<path d=\"M42 56 Q40 80 50 90 Q58 94 62 86 Q54 84 56 56 Z\" fill=\"#B0BEC5\"/>" +
        "<circle cx=\"40\" cy=\"40\" r=\"4\" fill=\"" + Rasgos + "\"/><circle cx=\"60\" cy=\"40\" r=\"4\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M38 60 Q32 66 34 72 M62 60 Q68 66 66 72\" stroke=\"#FFFDE7\" stroke-width=\"4\" fill=\"none\" stroke-linecap=\"round\"/>";

    private const string Mono =
        "<circle cx=\"20\" cy=\"50\" r=\"12\" fill=\"#8D6E63\"/><circle cx=\"80\" cy=\"50\" r=\"12\" fill=\"#8D6E63\"/>" +
        "<circle cx=\"20\" cy=\"50\" r=\"6\" fill=\"#D7B899\"/><circle cx=\"80\" cy=\"50\" r=\"6\" fill=\"#D7B899\"/>" +
        "<circle cx=\"50\" cy=\"50\" r=\"32\" fill=\"#8D6E63\"/>" +
        "<path d=\"M30 50 Q30 32 44 36 Q50 40 56 36 Q70 32 70 50 Q70 78 50 80 Q30 78 30 50 Z\" fill=\"#D7B899\"/>" +
        "<circle cx=\"42\" cy=\"48\" r=\"4\" fill=\"" + Rasgos + "\"/><circle cx=\"58\" cy=\"48\" r=\"4\" fill=\"" + Rasgos + "\"/>" +
        "<circle cx=\"47\" cy=\"60\" r=\"1.8\" fill=\"" + Rasgos + "\"/><circle cx=\"53\" cy=\"60\" r=\"1.8\" fill=\"" + Rasgos + "\"/>" +
        "<path d=\"M40 68 Q50 76 60 68\" stroke=\"" + Rasgos + "\" stroke-width=\"2.5\" fill=\"none\" stroke-linecap=\"round\"/>";

    // ---------- Cuerpo: la parte que se pregunta va en rojo con un anillo punteado ----------

    private static string Anillo(double cx, double cy, double r) =>
        $"<circle cx=\"{cx.ToString(CultureInfo.InvariantCulture)}\" cy=\"{cy.ToString(CultureInfo.InvariantCulture)}\" r=\"{r.ToString(CultureInfo.InvariantCulture)}\" fill=\"none\" stroke=\"{Resalte}\" stroke-width=\"2.5\" stroke-dasharray=\"5 4\"/>";

    private static string Color(string parte, string objetivo, string normal) => parte == objetivo ? Resalte : normal;

    private static string Cara(string parte)
    {
        var sb = new StringBuilder();
        sb.Append($"<circle cx=\"18\" cy=\"54\" r=\"8\" fill=\"{Color("ear", parte, Piel)}\"/>");
        sb.Append($"<circle cx=\"82\" cy=\"54\" r=\"8\" fill=\"{Color("ear", parte, Piel)}\"/>");
        sb.Append($"<ellipse cx=\"50\" cy=\"54\" rx=\"32\" ry=\"36\" fill=\"{Piel}\"{(parte == "face" ? $" stroke=\"{Resalte}\" stroke-width=\"4\"" : "")}/>");
        sb.Append($"<path d=\"M18 46 Q16 14 50 14 Q84 14 82 46 Q74 30 50 30 Q26 30 18 46 Z\" fill=\"{Color("hair", parte, "#5D4037")}\"/>");
        var ojo = Color("eye", parte, Rasgos);
        sb.Append($"<ellipse cx=\"38\" cy=\"52\" rx=\"5\" ry=\"6\" fill=\"{ojo}\"/><ellipse cx=\"62\" cy=\"52\" rx=\"5\" ry=\"6\" fill=\"{ojo}\"/>");
        sb.Append($"<path d=\"M50 56 Q44 68 50 70 Q54 70 54 68\" fill=\"{(parte == "nose" ? Resalte : "none")}\" stroke=\"{Color("nose", parte, "#C68A6A")}\" stroke-width=\"3\" stroke-linecap=\"round\"/>");
        sb.Append(parte == "mouth"
            ? $"<path d=\"M36 76 Q50 90 64 76 Z\" fill=\"{Resalte}\"/>"
            : "<path d=\"M38 77 Q50 86 62 77\" stroke=\"#B5543C\" stroke-width=\"3\" fill=\"none\" stroke-linecap=\"round\"/>");

        sb.Append(parte switch
        {
            "ear" => Anillo(18, 54, 13),
            "eye" => Anillo(62, 52, 11),
            "nose" => Anillo(51, 63, 11),
            "mouth" => Anillo(50, 80, 15),
            "hair" => Anillo(50, 24, 20),
            _ => ""
        });
        return sb.ToString();
    }

    private static string Figura(string parte)
    {
        var sb = new StringBuilder();
        var ropa = "#64B5F6";
        var pantalon = "#5C6BC0";
        // Piernas y pies
        sb.Append($"<rect x=\"39\" y=\"62\" width=\"9\" height=\"26\" rx=\"4\" fill=\"{Color("leg", parte, pantalon)}\"/>");
        sb.Append($"<rect x=\"52\" y=\"62\" width=\"9\" height=\"26\" rx=\"4\" fill=\"{Color("leg", parte, pantalon)}\"/>");
        sb.Append($"<ellipse cx=\"41\" cy=\"91\" rx=\"8\" ry=\"4.5\" fill=\"{Color("foot", parte, "#5D4037")}\"/>");
        sb.Append($"<ellipse cx=\"59\" cy=\"91\" rx=\"8\" ry=\"4.5\" fill=\"{Color("foot", parte, "#5D4037")}\"/>");
        // Brazos y manos
        sb.Append($"<rect x=\"24\" y=\"36\" width=\"8\" height=\"26\" rx=\"4\" fill=\"{Color("arm", parte, ropa)}\" transform=\"rotate(12 28 36)\"/>");
        sb.Append($"<rect x=\"68\" y=\"36\" width=\"8\" height=\"26\" rx=\"4\" fill=\"{Color("arm", parte, ropa)}\" transform=\"rotate(-12 72 36)\"/>");
        sb.Append($"<circle cx=\"23\" cy=\"64\" r=\"5.5\" fill=\"{Color("hand", parte, Piel)}\"/>");
        sb.Append($"<circle cx=\"77\" cy=\"64\" r=\"5.5\" fill=\"{Color("hand", parte, Piel)}\"/>");
        // Torso
        sb.Append($"<rect x=\"34\" y=\"32\" width=\"32\" height=\"34\" rx=\"10\" fill=\"{ropa}\"/>");
        // Cabeza
        sb.Append($"<circle cx=\"50\" cy=\"18\" r=\"13\" fill=\"{Color("head", parte, Piel)}\"/>");
        sb.Append("<path d=\"M37 16 Q38 4 50 4 Q62 4 63 16 Q56 10 50 10 Q44 10 37 16 Z\" fill=\"#5D4037\"/>");
        sb.Append($"<circle cx=\"45\" cy=\"19\" r=\"1.8\" fill=\"{Rasgos}\"/><circle cx=\"55\" cy=\"19\" r=\"1.8\" fill=\"{Rasgos}\"/>");
        sb.Append($"<path d=\"M46 24 Q50 27 54 24\" stroke=\"{Rasgos}\" stroke-width=\"1.5\" fill=\"none\" stroke-linecap=\"round\"/>");

        sb.Append(parte switch
        {
            "head" => Anillo(50, 16, 17),
            "hand" => Anillo(77, 64, 10),
            "arm" => Anillo(74, 48, 14),
            "leg" => Anillo(56, 74, 15),
            "foot" => Anillo(59, 91, 11),
            _ => ""
        });
        return sb.ToString();
    }

    private static string FiguraDeEspaldas()
    {
        // Vista de espaldas: la cabeza solo con cabello (sin cara) y la espalda resaltada.
        return
            "<rect x=\"39\" y=\"62\" width=\"9\" height=\"26\" rx=\"4\" fill=\"#5C6BC0\"/><rect x=\"52\" y=\"62\" width=\"9\" height=\"26\" rx=\"4\" fill=\"#5C6BC0\"/>" +
            "<ellipse cx=\"43\" cy=\"91\" rx=\"7\" ry=\"4\" fill=\"#5D4037\"/><ellipse cx=\"57\" cy=\"91\" rx=\"7\" ry=\"4\" fill=\"#5D4037\"/>" +
            "<rect x=\"24\" y=\"36\" width=\"8\" height=\"26\" rx=\"4\" fill=\"#64B5F6\" transform=\"rotate(12 28 36)\"/>" +
            "<rect x=\"68\" y=\"36\" width=\"8\" height=\"26\" rx=\"4\" fill=\"#64B5F6\" transform=\"rotate(-12 72 36)\"/>" +
            "<circle cx=\"23\" cy=\"64\" r=\"5.5\" fill=\"" + Piel + "\"/><circle cx=\"77\" cy=\"64\" r=\"5.5\" fill=\"" + Piel + "\"/>" +
            "<rect x=\"34\" y=\"32\" width=\"32\" height=\"34\" rx=\"10\" fill=\"" + Resalte + "\"/>" +
            "<circle cx=\"50\" cy=\"18\" r=\"13\" fill=\"#5D4037\"/>" +
            "<circle cx=\"37.5\" cy=\"19\" r=\"3\" fill=\"" + Piel + "\"/><circle cx=\"62.5\" cy=\"19\" r=\"3\" fill=\"" + Piel + "\"/>" +
            Anillo(50, 49, 22);
    }
}
