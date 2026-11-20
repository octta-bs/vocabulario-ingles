namespace VocabularioIngles.Data;

/// <summary>Una frase en pasado y otra en futuro para practicar un verbo.</summary>
public record FrasesVerbo(string Pasado, string PasadoEs, string Futuro, string FuturoEs);

/// <summary>
/// Frases escritas a mano para practicar los tiempos verbales. Se guardan aquí y no en la base
/// de datos para no tener que cambiar su estructura (y no perder el progreso guardado).
/// La frase en pasado debe contener el pasado del verbo, y la de futuro "will" + el verbo.
/// Conviene revisarlas, igual que la columna "Se lee".
/// </summary>
public static class FrasesVerbos
{
    private static readonly Dictionary<string, FrasesVerbo> Frases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["fix"] = new("Yesterday I fixed a bug on the login page.", "Ayer arreglé un error en la página de inicio de sesión.",
                      "Tomorrow I will fix the tests.", "Mañana arreglaré las pruebas."),
        ["write"] = new("Yesterday I wrote a test for the API.", "Ayer escribí una prueba para la API.",
                        "Tomorrow I will write the documentation.", "Mañana escribiré la documentación."),
        ["build"] = new("Last month we built a new website.", "El mes pasado construimos un sitio web nuevo.",
                        "Next week I will build the login page.", "La próxima semana construiré la página de inicio de sesión."),
        ["test"] = new("Yesterday I tested the new feature.", "Ayer probé la nueva función.",
                       "Tomorrow I will test the app on my phone.", "Mañana probaré la aplicación en mi teléfono."),
        ["run"] = new("Yesterday I ran the tests twice.", "Ayer ejecuté las pruebas dos veces.",
                      "Later I will run the program again.", "Más tarde ejecutaré el programa otra vez."),
        ["send"] = new("Yesterday I sent the report to my manager.", "Ayer envié el reporte a mi jefe.",
                       "Tomorrow I will send the file to the client.", "Mañana enviaré el archivo al cliente."),
        ["save"] = new("Last night I saved my work.", "Anoche guardé mi trabajo.",
                       "Later I will save the changes.", "Más tarde guardaré los cambios."),
        ["delete"] = new("Yesterday I deleted the old files.", "Ayer borré los archivos viejos.",
                         "Tomorrow I will delete the test data.", "Mañana borraré los datos de prueba."),
        ["open"] = new("This morning I opened the project.", "Esta mañana abrí el proyecto.",
                       "Tomorrow I will open a pull request.", "Mañana abriré un pull request."),
        ["learn"] = new("Last year I learned C#.", "El año pasado aprendí C#.",
                        "Next month I will learn a new framework.", "El próximo mes aprenderé un framework nuevo."),
        ["read"] = new("Yesterday I read the documentation.", "Ayer leí la documentación.",
                       "Tomorrow I will read your code.", "Mañana leeré tu código."),
        ["install"] = new("Yesterday I installed the new version.", "Ayer instalé la versión nueva.",
                          "Tomorrow I will install the database.", "Mañana instalaré la base de datos."),
        ["update"] = new("Last week we updated the server.", "La semana pasada actualizamos el servidor.",
                         "Tomorrow I will update the app.", "Mañana actualizaré la aplicación."),
        ["deploy"] = new("Yesterday we deployed the new version.", "Ayer publicamos la versión nueva.",
                         "On Friday I will deploy the app to the cloud.", "El viernes publicaré la aplicación en la nube.")
    };

    public static FrasesVerbo? Buscar(string verbo)
    {
        return Frases.TryGetValue(verbo, out var frases) ? frases : null;
    }
}
