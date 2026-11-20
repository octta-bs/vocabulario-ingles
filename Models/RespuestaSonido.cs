namespace VocabularioIngles.Models;

/// <summary>Los dos ejercicios de "Sonidos difíciles".</summary>
public enum EjercicioSonido
{
    // La app dice una palabra y eliges cuál de las dos oíste.
    Escuchar,

    // Dices la palabra y el reconocimiento de voz dice cuál entendió.
    Decir
}

/// <summary>Una respuesta de "Sonidos difíciles". Con estas se calcula si ya dominas cada sonido.</summary>
public class RespuestaSonido
{
    public int Id { get; set; }

    // La clave del sonido en Data/SonidosDificiles.cs ("i-corta").
    public string Sonido { get; set; } = "";

    public EjercicioSonido Ejercicio { get; set; }
    public bool Acerto { get; set; }

    // Cuándo respondiste (UTC).
    public DateTime Fecha { get; set; }
}
