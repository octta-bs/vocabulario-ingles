using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Palabras iniciales. La pronunciación (IPA, inglés de Estados Unidos) y las formas de los verbos
/// salen de los conjuntos de datos abiertos descritos en el README. La columna "ComoSeLee" es una
/// aproximación para hispanohablantes escrita a mano: conviene revisarla.
/// </summary>
public static class SeedData
{
    public static void Cargar(AppDbContext db)
    {
        CargarVocabularioInicial(db);
        CargarVocabularioGeneral(db);
        CargarFrasesDeTrabajo(db);
        CargarGramatica(db);
        ActualizarTextos(db);
    }

    /// <summary>
    /// Copia a la base los textos corregidos del código (traducción, pronunciación, "se lee", ejemplos, notas, la
    /// palabra clave y la frase con error), buscando cada palabra por su categoría y su inglés. Así una corrección
    /// del contenido llega también a una base que ya existe. No toca el progreso, ni las palabras que agregaste tú
    /// (por ejemplo, las de "Mis lecturas").
    /// </summary>
    private static void ActualizarTextos(AppDbContext db)
    {
        var semillas = Semillas()
            .GroupBy(p => (p.Categoria!.Nombre, p.Ingles.ToLowerInvariant()))
            .ToDictionary(g => g.Key, g => g.First());
        var categorias = db.Categorias.ToDictionary(c => c.Id, c => c.Nombre);

        foreach (var palabra in db.Palabras)
        {
            if (!semillas.TryGetValue((categorias[palabra.CategoriaId], palabra.Ingles.ToLowerInvariant()), out var semilla))
            {
                continue;
            }

            palabra.Espanol = semilla.Espanol;
            palabra.Pronunciacion = semilla.Pronunciacion;
            palabra.ComoSeLee = semilla.ComoSeLee;
            palabra.Ejemplo = semilla.Ejemplo;
            palabra.EjemploEs = semilla.EjemploEs;
            palabra.Nota = semilla.Nota;
            palabra.PalabraClave = semilla.PalabraClave;
            palabra.FraseIncorrecta = semilla.FraseIncorrecta;
            palabra.Pasado = semilla.Pasado;
            palabra.Participio = semilla.Participio;
        }

        // Solo guarda si algo cambió de verdad (EF compara los valores).
        db.SaveChanges();
    }

    /// <summary>Todas las palabras del contenido de la app, tal como están en el código (sin guardar).</summary>
    private static IEnumerable<Palabra> Semillas() =>
        PalabrasIniciales().Concat(PalabrasGenerales()).Concat(FrasesDeTrabajo()).Concat(FrasesDeGramatica());

    /// <summary>
    /// Agrega cada tema de <see cref="TemasGramatica"/> que todavía no exista, como una categoría con sus frases
    /// de tipo gramática. No entran al repaso hasta que eliges practicar el tema.
    /// </summary>
    private static void CargarGramatica(AppDbContext db)
    {
        var existentes = db.Categorias.ToDictionary(c => c.Nombre);

        foreach (var tema in TemasGramatica.Todos)
        {
            if (existentes.TryGetValue(tema.Categoria, out var existente))
            {
                // Si el tema cambió de nivel, se actualiza (sin tocar sus frases ni el progreso).
                existente.Nivel = tema.Nivel;
                continue;
            }

            db.Palabras.AddRange(FrasesDeGramatica().Where(p => p.Categoria!.Nombre == tema.Categoria));
        }

        db.SaveChanges();
    }

    private static IEnumerable<Palabra> FrasesDeGramatica()
    {
        foreach (var tema in TemasGramatica.Todos)
        {
            var categoria = new Categoria { Nombre = tema.Categoria, Nivel = tema.Nivel };
            foreach (var f in tema.Frases)
            {
                yield return new Palabra
                {
                    Categoria = categoria,
                    Tipo = TipoPalabra.Gramatica,
                    Ingles = f.Correcta,
                    Espanol = f.Espanol,
                    FraseIncorrecta = f.Incorrecta,
                    PalabraClave = f.PalabraClave,
                    Nota = f.Por
                };
            }
        }
    }

    /// <summary>
    /// Agrega cada categoría de <see cref="VocabularioGeneral"/> (Pre A1 a A2) que todavía no exista,
    /// sin tocar las demás ni el progreso.
    /// </summary>
    private static void CargarVocabularioGeneral(AppDbContext db)
    {
        var existentes = db.Categorias.Select(c => c.Nombre).ToHashSet();

        foreach (var semilla in VocabularioGeneral.Categorias)
        {
            if (existentes.Contains(semilla.Nombre))
            {
                continue;
            }

            db.Palabras.AddRange(PalabrasGenerales().Where(p => p.Categoria!.Nombre == semilla.Nombre));
        }

        db.SaveChanges();
    }

    private static IEnumerable<Palabra> PalabrasGenerales()
    {
        foreach (var semilla in VocabularioGeneral.Categorias)
        {
            var categoria = new Categoria { Nombre = semilla.Nombre, Nivel = semilla.Nivel };
            foreach (var p in semilla.Palabras)
            {
                yield return new Palabra
                {
                    Categoria = categoria,
                    Tipo = p.Pasado == null ? TipoPalabra.Sustantivo : TipoPalabra.Verbo,
                    Ingles = p.Ingles,
                    Espanol = p.Espanol,
                    Pronunciacion = p.Pronunciacion,
                    ComoSeLee = p.ComoSeLee,
                    Pasado = p.Pasado,
                    Participio = p.Participio,
                    Ejemplo = p.Ejemplo,
                    EjemploEs = p.EjemploEs
                };
            }
        }
    }

    /// <summary>
    /// Agrega cada categoría de <see cref="FrasesTrabajo"/> que todavía no exista. Así una base que ya
    /// tiene progreso recibe las frases nuevas sin tocar lo demás.
    /// </summary>
    private static void CargarFrasesDeTrabajo(AppDbContext db)
    {
        var existentes = db.Categorias.Select(c => c.Nombre).ToHashSet();

        foreach (var (nombre, frases) in FrasesTrabajo.PorCategoria)
        {
            if (existentes.Contains(nombre))
            {
                continue;
            }

            db.Palabras.AddRange(FrasesDeTrabajo().Where(p => p.Categoria!.Nombre == nombre));
        }

        db.SaveChanges();
    }

    private static IEnumerable<Palabra> FrasesDeTrabajo()
    {
        foreach (var (nombre, frases) in FrasesTrabajo.PorCategoria)
        {
            var categoria = new Categoria { Nombre = nombre, Nivel = FrasesTrabajo.Nivel(nombre) };
            foreach (var f in frases)
            {
                yield return new Palabra
                {
                    Categoria = categoria,
                    Tipo = TipoPalabra.Frase,
                    Ingles = f.Ingles,
                    Espanol = f.Espanol,
                    PalabraClave = f.PalabraClave,
                    Nota = f.Nota
                };
            }
        }
    }

    private static void CargarVocabularioInicial(AppDbContext db)
    {
        if (db.Palabras.Any())
        {
            return;
        }

        db.Palabras.AddRange(PalabrasIniciales());
        db.SaveChanges();
    }

    private static List<Palabra> PalabrasIniciales()
    {
        var tec = new Categoria { Nombre = "Tecnología" };
        var trabajo = new Categoria { Nombre = "Trabajo" };
        var verbos = new Categoria { Nombre = "Verbos de programación" };

        return new List<Palabra>
        {
            Sustantivo(tec, "code", "código", "/ˈkoʊd/", "cóud", "I write code every day.", "Escribo código todos los días."),
            Sustantivo(tec, "bug", "error (en un programa)", "/ˈbəɡ/", "bág", "There is a bug in the database.", "Hay un error en la base de datos."),
            Sustantivo(tec, "database", "base de datos", "/ˈdeɪtəˌbeɪs/", "déi-ta-beis", "The database is on the server.", "La base de datos está en el servidor."),
            Sustantivo(tec, "server", "servidor", "/ˈsɝvɝ/", "sér-ver", "The server is down.", "El servidor está caído."),
            Sustantivo(tec, "software", "software", "/ˈsɔftˌwɛɹ/", "sóft-uer", "We build software for small businesses.", "Hacemos software para negocios pequeños."),
            Sustantivo(tec, "website", "sitio web", "/ˈwɛbˌsaɪt/", "uéb-sáit", "The website is very fast.", "El sitio web es muy rápido."),
            Sustantivo(tec, "file", "archivo", "/ˈfaɪɫ/", "fáil", "I send the file to the client.", "Envío el archivo al cliente."),
            Sustantivo(tec, "password", "contraseña", "/ˈpæsˌwɝd/", "pás-uerd", "My password is very long.", "Mi contraseña es muy larga."),
            Sustantivo(tec, "network", "red", "/ˈnɛtˌwɝk/", "nét-uerk", "The network is slow today.", "La red está lenta hoy."),
            Sustantivo(tec, "screen", "pantalla", "/ˈskɹin/", "scrín", "I see an error on the screen.", "Veo un error en la pantalla."),
            Sustantivo(tec, "error", "error", "/ˈɛɹɝ/", "é-rer", "The error is on line ten.", "El error está en la línea diez."),
            Sustantivo(tec, "cloud", "nube", "/ˈkɫaʊd/", "cláud", "The app runs in the cloud.", "La aplicación corre en la nube."),
            Sustantivo(trabajo, "task", "tarea", "/ˈtæsk/", "task", "This task is easy.", "Esta tarea es fácil."),
            Sustantivo(trabajo, "meeting", "reunión", "/ˈmitɪŋ/", "mí-ting", "We have a meeting at ten.", "Tenemos una reunión a las diez."),
            Sustantivo(trabajo, "deadline", "fecha límite", "/ˈdɛdˌɫaɪn/", "déd-láin", "The deadline is on Friday.", "La fecha límite es el viernes."),
            Sustantivo(trabajo, "team", "equipo", "/ˈtim/", "tím", "I work with a good team.", "Trabajo con un buen equipo."),
            Sustantivo(trabajo, "manager", "jefe, gerente", "/ˈmænədʒɝ/", "má-ne-yer", "My manager is in the meeting.", "Mi jefe está en la reunión."),
            Sustantivo(trabajo, "schedule", "horario", "/ˈskɛdʒuɫ/", "ské-yul", "What is the schedule for today?", "¿Cuál es el horario de hoy?"),
            Sustantivo(trabajo, "report", "reporte, informe", "/ɹiˈpɔɹt/", "ri-pórt", "I send the report every Friday.", "Envío el reporte todos los viernes."),
            Sustantivo(trabajo, "client", "cliente", "/ˈkɫaɪənt/", "cláy-ent", "The client wants a new website.", "El cliente quiere un sitio web nuevo."),
            Sustantivo(trabajo, "project", "proyecto", "/ˈpɹɑdʒɛkt/", "prá-yect", "I work on a web project.", "Trabajo en un proyecto web."),
            Verbo(verbos, "fix", "arreglar", "/ˈfɪks/", "fíks", "fixed", "fixed", "I fix bugs.", "Arreglo errores."),
            Verbo(verbos, "write", "escribir", "/ˈɹaɪt/", "ráit", "wrote", "written", "I write code every day.", "Escribo código todos los días."),
            Verbo(verbos, "build", "construir", "/ˈbɪɫd/", "bíld", "built", "built", "I build web apps.", "Construyo aplicaciones web."),
            Verbo(verbos, "test", "probar", "/ˈtɛst/", "tést", "tested", "tested", "I test the app before the meeting.", "Pruebo la aplicación antes de la reunión."),
            Verbo(verbos, "run", "ejecutar, correr", "/ˈɹən/", "rán", "ran", "run", "I run the program.", "Ejecuto el programa."),
            Verbo(verbos, "send", "enviar", "/ˈsɛnd/", "sénd", "sent", "sent", "I send the file to the client.", "Envío el archivo al cliente."),
            Verbo(verbos, "save", "guardar", "/ˈseɪv/", "séiv", "saved", "saved", "I save my work.", "Guardo mi trabajo."),
            Verbo(verbos, "delete", "borrar", "/dɪˈɫit/", "di-lít", "deleted", "deleted", "I delete the old file.", "Borro el archivo viejo."),
            Verbo(verbos, "open", "abrir", "/ˈoʊpən/", "óu-pen", "opened", "opened", "I open the project.", "Abro el proyecto."),
            Verbo(verbos, "learn", "aprender", "/ˈɫɝn/", "lérn", "learned", "learned", "I learn new words every day.", "Aprendo palabras nuevas cada día.", "En inglés de Estados Unidos el pasado más común es learned (también existe learnt)."),
            Verbo(verbos, "read", "leer", "/ˈɹid/", "ríd", "read", "read", "I read the documentation.", "Leo la documentación.", "El pasado se escribe igual (read), pero se pronuncia 'red'."),
            Verbo(verbos, "install", "instalar", "/ˌɪnˈstɔɫ/", "in-stól", "installed", "installed", "I install the software.", "Instalo el software."),
            Verbo(verbos, "update", "actualizar", "/əpˈdeɪt/", "ap-déit", "updated", "updated", "I update the database.", "Actualizo la base de datos."),
            Verbo(verbos, "deploy", "publicar (una aplicación)", "/dɪˈpɫɔɪ/", "di-plói", "deployed", "deployed", "I deploy the app to the cloud.", "Publico la aplicación en la nube.")
        };
    }

    private static Palabra Sustantivo(Categoria categoria, string ingles, string espanol,
        string pronunciacion, string comoSeLee, string ejemplo, string ejemploEs)
    {
        return new Palabra
        {
            Categoria = categoria,
            Tipo = TipoPalabra.Sustantivo,
            Ingles = ingles,
            Espanol = espanol,
            Pronunciacion = pronunciacion,
            ComoSeLee = comoSeLee,
            Ejemplo = ejemplo,
            EjemploEs = ejemploEs
        };
    }

    private static Palabra Verbo(Categoria categoria, string ingles, string espanol,
        string pronunciacion, string comoSeLee, string pasado, string participio,
        string ejemplo, string ejemploEs, string? nota = null)
    {
        return new Palabra
        {
            Categoria = categoria,
            Tipo = TipoPalabra.Verbo,
            Ingles = ingles,
            Espanol = espanol,
            Pronunciacion = pronunciacion,
            ComoSeLee = comoSeLee,
            Pasado = pasado,
            Participio = participio,
            Ejemplo = ejemplo,
            EjemploEs = ejemploEs,
            Nota = nota
        };
    }
}
