using System.Text.RegularExpressions;
using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class SonidosTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly SonidosService _sonidos;

    public SonidosTests()
    {
        _sonidos = new SonidosService(_base.Db, TimeProvider.System);
    }

    public void Dispose() => _base.Dispose();

    [Fact]
    public void Los_sonidos_son_validos()
    {
        Assert.Equal(10, SonidosDificiles.Todos.Length);
        Assert.Equal(SonidosDificiles.Todos.Length, SonidosDificiles.Todos.Select(s => s.Clave).Distinct().Count());

        foreach (var sonido in SonidosDificiles.Todos)
        {
            Assert.InRange(sonido.Pares.Length, 3, 6);
            Assert.NotEmpty(sonido.ComoSeHace);
            Assert.All(sonido.Pares, par =>
            {
                // Palabras sueltas en minúsculas (así las compara el reconocimiento de voz) y distintas entre sí.
                Assert.Matches(new Regex("^[a-z]+$"), par.Con);
                Assert.Matches(new Regex("^[a-z]+$"), par.Contra);
                Assert.NotEqual(par.Con, par.Contra);
            });
            // Dentro de un sonido, ninguna palabra se repite.
            var palabras = sonido.Pares.SelectMany(p => new[] { p.Con, p.Contra }).ToList();
            Assert.Equal(palabras.Count, palabras.Distinct().Count());
        }
    }

    [Fact]
    public async Task Al_principio_nada_esta_empezado_y_sigue_el_primero()
    {
        var sonidos = await _sonidos.ListarAsync();

        Assert.Equal(Enumerable.Range(1, 10), sonidos.Select(s => s.Numero));
        Assert.All(sonidos, s => Assert.False(s.Empezado));
        Assert.Equal("e-inicial", SonidosService.Siguiente(sonidos)!.Sonido.Clave);
    }

    [Fact]
    public async Task Se_domina_con_8_de_las_ultimas_10_y_entonces_sigue_el_siguiente()
    {
        // 7 aciertos y 3 fallos: todavía no.
        for (var i = 0; i < 10; i++)
        {
            await _sonidos.GuardarAsync("e-inicial", EjercicioSonido.Escuchar, acerto: i < 7);
        }
        var estado = (await _sonidos.ObtenerAsync("e-inicial"))!;
        Assert.Equal((10, 7, false), (estado.Respondidas, estado.Aciertos, estado.Dominado));

        // Las 3 fallas son las más recientes. Con 7 aciertos nuevos, las últimas 10 son 7 aciertos y 3 fallas: aún no.
        for (var i = 0; i < 7; i++)
        {
            estado = (await _sonidos.GuardarAsync("e-inicial", EjercicioSonido.Decir, true))!;
        }
        Assert.Equal((7, false), (estado.Aciertos, estado.Dominado));

        // El octavo saca una falla de la ventana: 8 de 10.
        estado = (await _sonidos.GuardarAsync("e-inicial", EjercicioSonido.Decir, true))!;
        Assert.Equal(8, estado.Aciertos);
        Assert.True(estado.Dominado);
        Assert.Equal("v-b", SonidosService.Siguiente(await _sonidos.ListarAsync())!.Sonido.Clave);
    }

    [Fact]
    public async Task Un_sonido_que_no_existe_no_se_guarda()
    {
        Assert.Null(await _sonidos.GuardarAsync("no-existe", EjercicioSonido.Escuchar, true));
        Assert.Empty(_base.Db.RespuestasSonido);
    }
}
