using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>
/// Explica por qué una respuesta está mal, con reglas sencillas y sin IA: nada escrito, respuesta en español,
/// tiempo verbal equivocado, plural, una letra de diferencia, otra palabra que existe en la app o, en frases,
/// qué palabras faltaron y cuáles sobraron. Si ninguna regla aplica, no explica nada.
/// </summary>
public static class ExplicadorErrores
{
    /// <param name="ejercicio">El ejercicio que se respondió (trae la respuesta correcta y la palabra).</param>
    /// <param name="escrita">Lo que escribió la persona.</param>
    /// <param name="otrasPalabras">Las palabras de la app (inglés y español), para detectar si escribió otra.</param>
    public static List<string> Explicar(Ejercicio ejercicio, string? escrita,
        IEnumerable<(string Ingles, string Espanol)> otrasPalabras)
    {
        var explicaciones = new List<string>();
        var dada = ComparadorRespuestas.Normalizar(escrita);
        var correcta = ComparadorRespuestas.Normalizar(ejercicio.Respuesta);
        if (dada == correcta)
        {
            return explicaciones;
        }

        if (dada.Length == 0)
        {
            explicaciones.Add("No escribiste nada. Si no la recuerdas, intenta escribir lo que creas: fallar también ayuda a recordarla.");
            return explicaciones;
        }

        // En "Corrige la frase", si la dejó igual, el error sigue ahí.
        if (ejercicio.Tipo == TipoEjercicio.CorregirFrase
            && dada == ComparadorRespuestas.Normalizar(ejercicio.Palabra.FraseIncorrecta))
        {
            explicaciones.Add("Escribiste la frase igual que estaba: el error sigue ahí.");
            return explicaciones;
        }

        // Si escribió otra palabra en inglés que existe en la app, eso explica más que cualquier otra regla
        // (por ejemplo, "error" en vez de "bug", aunque "error" también aparezca en su traducción).
        var otra = otrasPalabras.FirstOrDefault(o => ComparadorRespuestas.Normalizar(o.Ingles) == dada);
        if (otra.Ingles != null)
        {
            explicaciones.Add($"Escribiste \"{otra.Ingles}\" ({otra.Espanol}), que es otra palabra.");
            return explicaciones;
        }

        var palabra = ejercicio.Palabra;
        if (dada == ComparadorRespuestas.Normalizar(palabra.Espanol) || EsAlgunaTraduccion(dada, palabra.Espanol))
        {
            explicaciones.Add("Escribiste la palabra en español; aquí se pide en inglés.");
            return explicaciones;
        }

        if (palabra.EsVerbo && TiempoVerbal(dada, palabra) is { } tiempoEscrito
            && TiempoVerbal(correcta, palabra) is { } tiempoCorrecto && tiempoEscrito != tiempoCorrecto)
        {
            explicaciones.Add($"Escribiste el {tiempoEscrito} ({escrita!.Trim()}); aquí va el {tiempoCorrecto} ({ejercicio.Respuesta}).");
            return explicaciones;
        }

        var dadas = dada.Split(' ');
        var correctas = correcta.Split(' ');
        if (dadas.Length == 1 && correctas.Length == 1)
        {
            explicaciones.AddRange(ExplicarUnaPalabra(dada, correcta));
        }
        else
        {
            explicaciones.AddRange(ExplicarFrase(dadas, correctas));
        }

        return explicaciones;
    }

    /// <summary>"jefe, gerente" o "resfriado; frío": acepta cualquiera de las traducciones.</summary>
    private static bool EsAlgunaTraduccion(string dada, string espanol)
    {
        return espanol.Split(',', ';', '(', ')')
            .Select(t => ComparadorRespuestas.Normalizar(t))
            .Any(t => t.Length > 0 && t == dada);
    }

    private static string? TiempoVerbal(string texto, Palabra verbo)
    {
        var presente = ComparadorRespuestas.Normalizar(verbo.Ingles);
        var pasado = ComparadorRespuestas.Normalizar(verbo.Pasado);
        var participio = ComparadorRespuestas.Normalizar(verbo.Participio);

        // Una forma que se escribe igual en dos tiempos (read/read) no permite saber cuál quiso decir.
        if (texto == presente && texto != pasado)
        {
            return "presente";
        }

        if (texto == pasado && texto != presente)
        {
            return "pasado";
        }

        if (texto == "will " + presente)
        {
            return "futuro";
        }

        if (texto == participio && texto != pasado && texto != presente)
        {
            return "participio";
        }

        return null;
    }

    private static IEnumerable<string> ExplicarUnaPalabra(string dada, string correcta)
    {
        if (dada + "s" == correcta)
        {
            yield return "Te faltó la \"s\" del final.";
            yield break;
        }

        if (dada == correcta + "s")
        {
            yield return "Te sobró la \"s\" del final.";
            yield break;
        }

        if (DiferenciaDeUnaLetra(dada, correcta) is { } diferencia)
        {
            yield return $"Casi: {diferencia}. Revisa la ortografía.";
            yield break;
        }

        if (Distancia(dada, correcta) == 2 && correcta.Length >= 5)
        {
            yield return "Casi: te equivocaste en dos letras. Revisa la ortografía.";
        }
    }

    /// <summary>Si las palabras difieren en una sola letra, dice cuál (faltó, sobró, cambiada o volteada).</summary>
    private static string? DiferenciaDeUnaLetra(string dada, string correcta)
    {
        if (dada.Length + 1 == correcta.Length)
        {
            for (var i = 0; i < correcta.Length; i++)
            {
                if (correcta.Remove(i, 1) == dada)
                {
                    return $"te faltó la \"{correcta[i]}\"";
                }
            }
        }
        else if (dada.Length == correcta.Length + 1)
        {
            for (var i = 0; i < dada.Length; i++)
            {
                if (dada.Remove(i, 1) == correcta)
                {
                    return $"te sobró una \"{dada[i]}\"";
                }
            }
        }
        else if (dada.Length == correcta.Length)
        {
            var distintas = Enumerable.Range(0, dada.Length).Where(i => dada[i] != correcta[i]).ToList();
            if (distintas.Count == 1)
            {
                var i = distintas[0];
                return $"pusiste \"{dada[i]}\" en lugar de \"{correcta[i]}\"";
            }

            if (distintas.Count == 2 && distintas[1] == distintas[0] + 1
                && dada[distintas[0]] == correcta[distintas[1]] && dada[distintas[1]] == correcta[distintas[0]])
            {
                return $"volteaste las letras \"{correcta[distintas[0]]}{correcta[distintas[1]]}\"";
            }
        }

        return null;
    }

    /// <summary>En frases: qué palabras faltaron y cuáles sobraron (respetando las repetidas).</summary>
    private static IEnumerable<string> ExplicarFrase(string[] dadas, string[] correctas)
    {
        var sobrantes = dadas.ToList();
        var faltantes = new List<string>();
        foreach (var palabra in correctas)
        {
            if (!sobrantes.Remove(palabra))
            {
                faltantes.Add(palabra);
            }
        }

        if (faltantes.Count > 0)
        {
            yield return $"Te faltó: {string.Join(", ", faltantes.Select(p => $"\"{p}\""))}.";
        }

        if (sobrantes.Count > 0)
        {
            yield return $"Sobró: {string.Join(", ", sobrantes.Select(p => $"\"{p}\""))}.";
        }

        if (faltantes.Count == 0 && sobrantes.Count == 0)
        {
            yield return "Tienes todas las palabras, pero en otro orden.";
        }
    }

    /// <summary>Distancia de edición (cuántas letras hay que cambiar, poner o quitar).</summary>
    private static int Distancia(string a, string b)
    {
        var previa = Enumerable.Range(0, b.Length + 1).ToArray();
        for (var i = 1; i <= a.Length; i++)
        {
            var actual = new int[b.Length + 1];
            actual[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var costo = a[i - 1] == b[j - 1] ? 0 : 1;
                actual[j] = Math.Min(Math.Min(actual[j - 1] + 1, previa[j] + 1), previa[j - 1] + costo);
            }

            previa = actual;
        }

        return previa[b.Length];
    }
}
