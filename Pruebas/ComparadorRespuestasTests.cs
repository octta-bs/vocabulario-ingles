using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public class ComparadorRespuestasTests
{
    [Theory]
    [InlineData("database", "database")]
    [InlineData("  Database ", "database")]                                   // mayúsculas y espacios
    [InlineData("the server is down", "The server is down.")]                // puntuación
    [InlineData("today i am working on the new api", "Today I'm working on the new API.")]
    [InlineData("Today I’m working on the new API", "Today I'm working on the new API.")] // apóstrofo curvo
    [InlineData("I do not have any blockers", "I don't have any blockers.")]
    [InlineData("good catch i will fix it", "Good catch, I'll fix it.")]
    [InlineData("I can not go", "I can't go")]
    [InlineData("¿Could you repeat the question, please?", "Could you repeat the question, please?")]
    public void Acepta_respuestas_equivalentes(string escrita, string correcta)
    {
        Assert.True(ComparadorRespuestas.SonIguales(escrita, correcta));
    }

    [Theory]
    [InlineData("Today I working on the new API", "Today I'm working on the new API.")]
    [InlineData("databse", "database")]
    [InlineData("", "database")]
    [InlineData(null, "database")]
    [InlineData("fixed", "fix")]
    public void Rechaza_respuestas_distintas(string? escrita, string correcta)
    {
        Assert.False(ComparadorRespuestas.SonIguales(escrita, correcta));
    }

    [Fact]
    public void No_expande_contracciones_dentro_de_otras_palabras()
    {
        // "it's" sí se expande, pero no debe tocar palabras que solo lo contienen.
        Assert.Equal("it is fine", ComparadorRespuestas.Normalizar("It's fine."));
        Assert.Equal("visit", ComparadorRespuestas.Normalizar("visit"));
    }
}
