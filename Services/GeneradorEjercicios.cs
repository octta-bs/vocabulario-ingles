using System.Text.RegularExpressions;
using VocabularioIngles.Data;
using VocabularioIngles.Models;

namespace VocabularioIngles.Services;

/// <summary>
/// Arma los ejercicios del repaso a partir de una palabra: escribirla, o completar una frase
/// (y, si es verbo, también en pasado y en futuro).
/// </summary>
public class GeneradorEjercicios
{
    public const string Hueco = "___";

    /// <summary>Elige al azar uno de los ejercicios que se pueden hacer con esta palabra.</summary>
    public Ejercicio Elegir(Palabra palabra)
    {
        var posibles = Posibles(palabra);
        return posibles[Random.Shared.Next(posibles.Count)];
    }

    /// <summary>
    /// Vuelve a armar un ejercicio concreto (por ejemplo, al revisar la respuesta).
    /// Si ese tipo no se puede hacer con la palabra, regresa "escribir la palabra".
    /// </summary>
    public Ejercicio Crear(Palabra palabra, TipoEjercicio tipo)
    {
        return Posibles(palabra).FirstOrDefault(e => e.Tipo == tipo) ?? EscribirPalabra(palabra);
    }

    public List<Ejercicio> Posibles(Palabra palabra)
    {
        var lista = new List<Ejercicio> { EscribirPalabra(palabra) };

        var presente = Completar(palabra, TipoEjercicio.CompletarFrase, palabra.Ejemplo, palabra.EjemploEs,
            palabra.Ingles, palabra.EsVerbo ? "presente" : null);
        if (presente != null)
        {
            lista.Add(presente);
        }

        if (palabra.EsVerbo && FrasesVerbos.Buscar(palabra.Ingles) is { } frases)
        {
            if (!string.IsNullOrEmpty(palabra.Pasado)
                && Completar(palabra, TipoEjercicio.CompletarPasado, frases.Pasado, frases.PasadoEs,
                    palabra.Pasado, "pasado") is { } pasado)
            {
                lista.Add(pasado);
            }

            if (Completar(palabra, TipoEjercicio.CompletarFuturo, frases.Futuro, frases.FuturoEs,
                    palabra.Ingles, "futuro") is { } futuro)
            {
                lista.Add(futuro);
            }
        }

        return lista;
    }

    private static Ejercicio EscribirPalabra(Palabra palabra)
    {
        return new Ejercicio
        {
            Palabra = palabra,
            Tipo = TipoEjercicio.EscribirPalabra,
            Respuesta = palabra.Ingles
        };
    }

    /// <summary>
    /// Quita la palabra buscada de la frase y deja el hueco. Si la palabra no aparece como palabra
    /// completa (por ejemplo, "fix" dentro de "fixed"), no se puede armar el ejercicio.
    /// </summary>
    private static Ejercicio? Completar(Palabra palabra, TipoEjercicio tipo, string frase, string fraseEs,
        string buscada, string? tiempo)
    {
        if (string.IsNullOrWhiteSpace(frase) || string.IsNullOrWhiteSpace(buscada))
        {
            return null;
        }

        var patron = new Regex(@"\b" + Regex.Escape(buscada) + @"\b", RegexOptions.IgnoreCase);
        var encontrada = patron.Match(frase);
        if (!encontrada.Success)
        {
            return null;
        }

        return new Ejercicio
        {
            Palabra = palabra,
            Tipo = tipo,
            // Se pide tal como aparece en la frase (por si empieza con mayúscula).
            Respuesta = encontrada.Value,
            FraseConHueco = patron.Replace(frase, Hueco, 1),
            FraseCompleta = frase,
            FraseEs = fraseEs,
            TiempoVerbal = tiempo
        };
    }
}
