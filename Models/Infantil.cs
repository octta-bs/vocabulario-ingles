namespace VocabularioIngles.Models;

/// <summary>El perfil del niño o la niña que usa el modo infantil (solo hay uno).</summary>
public class PerfilInfantil
{
    public const int LargoMaximoNombre = 40;

    public int Id { get; set; }

    // Opcional: solo para saludarle ("¡Hola, Ana!"). Se guarda en tu base, no en el repositorio.
    public string Nombre { get; set; } = "";
}

/// <summary>
/// Cómo le va al niño con una palabra en el juego "Escucha y toca". Está separado del progreso
/// del adulto: no toca el repaso, la meta ni el historial.
/// </summary>
public class ProgresoInfantil
{
    // Con 3 aciertos seguidos la palabra cuenta como aprendida.
    public const int AciertosParaAprender = 3;

    public int Id { get; set; }

    public int PalabraId { get; set; }
    public Palabra? Palabra { get; set; }

    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int AciertosSeguidos { get; set; }

    // Última vez que se jugó (UTC).
    public DateTime UltimaVez { get; set; }

    public bool Aprendida => AciertosSeguidos >= AciertosParaAprender;
}

/// <summary>Una categoría del modo infantil, con cuántas palabras ya aprendió el niño.</summary>
public record CategoriaInfantil(int Id, string Nombre, int Total, int Aprendidas, string PalabraMuestra);

/// <summary>Una palabra con su dibujo y cómo le va al niño con ella.</summary>
public record PalabraInfantil(int Id, string Ingles, string Espanol, bool Aprendida);

/// <summary>Una ronda de "Escucha y toca": la palabra que se dice y las 3 opciones (ya revueltas).</summary>
public record RondaInfantil(int PalabraId, string Ingles, List<PalabraInfantil> Opciones);
