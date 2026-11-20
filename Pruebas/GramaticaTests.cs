using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using VocabularioIngles.Data;
using VocabularioIngles.Models;
using VocabularioIngles.Services;

namespace VocabularioIngles.Pruebas;

public sealed class GramaticaTests : IDisposable
{
    private readonly BaseDePrueba _base = new();
    private readonly GramaticaService _gramatica;

    public GramaticaTests()
    {
        _gramatica = new GramaticaService(_base.Db, TimeProvider.System);
    }

    public void Dispose() => _base.Dispose();

    private RepasoService Repaso() => new(_base.Db, new IntervalosAdaptativos(), new GeneradorEjercicios(),
        new RitmoPorAciertos(), BaseDePrueba.Zona(TimeProvider.System));

    private Palabra Frase(string correcta) => _base.Db.Palabras.Single(p => p.Ingles == correcta);

    [Fact]
    public void Los_temas_son_validos()
    {
        Assert.Equal(10, TemasGramatica.Todos.Length);
        Assert.Equal(TemasGramatica.Todos.Length, TemasGramatica.Todos.Select(t => t.Clave).Distinct().Count());

        foreach (var tema in TemasGramatica.Todos)
        {
            Assert.InRange(tema.Frases.Length, 6, 8);
            Assert.NotEmpty(tema.Regla);
            Assert.All(tema.Frases, f =>
            {
                // La versión con el error de verdad es distinta (no solo en mayúsculas, puntos o contracciones).
                Assert.NotEqual(ComparadorRespuestas.Normalizar(f.Correcta), ComparadorRespuestas.Normalizar(f.Incorrecta));
                // La parte gramatical aparece completa en la frase correcta, para el ejercicio de completar.
                Assert.Matches(new Regex(@"\b" + Regex.Escape(f.PalabraClave) + @"\b", RegexOptions.IgnoreCase), f.Correcta);
                // Un acento en el inglés sería una traducción en el lugar equivocado.
                Assert.DoesNotMatch(new Regex("[áéíóúñ¿¡]"), f.Correcta + f.Incorrecta);
                Assert.False(string.IsNullOrWhiteSpace(f.Espanol));
                Assert.False(string.IsNullOrWhiteSpace(f.Por));
            });
        }
    }

    [Fact]
    public async Task Los_temas_van_de_facil_a_avanzado_y_sigue_el_primero_sin_empezar()
    {
        var niveles = TemasGramatica.Todos.Select(t => t.Nivel).ToList();
        Assert.Equal(niveles.Order(), niveles);
        Assert.Equal("errores-tipicos", TemasGramatica.Todos[0].Clave);

        var temas = await _gramatica.ListarAsync();
        Assert.Equal(Enumerable.Range(1, temas.Count), temas.Select(t => t.Numero));
        Assert.Equal(1, GramaticaService.Siguiente(temas)!.Numero);

        await _gramatica.PracticarAsync("errores-tipicos");
        Assert.Equal(2, GramaticaService.Siguiente(await _gramatica.ListarAsync())!.Numero);
    }

    [Fact]
    public void Una_base_vieja_recibe_el_nivel_nuevo_del_tema()
    {
        var categoria = _base.Db.Categorias.Single(c => c.Nombre == TemasGramatica.Buscar("errores-tipicos")!.Categoria);
        categoria.Nivel = NivelMcer.B1;
        _base.Db.SaveChanges();

        SeedData.Cargar(_base.Db);

        Assert.Equal(NivelMcer.A2, _base.Db.Categorias.Single(c => c.Id == categoria.Id).Nivel);
    }

    [Fact]
    public void Las_frases_se_cargan_como_gramatica_del_modo_trabajo_y_sin_duplicar()
    {
        SeedData.Cargar(_base.Db);

        var frases = _base.Db.Palabras.Include(p => p.Categoria).Where(p => p.Tipo == TipoPalabra.Gramatica).ToList();
        Assert.Equal(TemasGramatica.Todos.Sum(t => t.Frases.Length), frases.Count);
        Assert.All(frases, p =>
        {
            Assert.StartsWith(TemasGramatica.PrefijoCategoria, p.Categoria!.Nombre);
            Assert.Equal(ModoApp.Trabajo, Modos.De(p.Categoria.Nombre));
            Assert.False(string.IsNullOrEmpty(p.FraseIncorrecta));
            Assert.Null(p.Progreso);
        });
    }

    [Fact]
    public async Task Las_palabras_nuevas_del_dia_no_meten_gramatica()
    {
        var repaso = Repaso();
        var sinEmpezar = await repaso.ContarSinEmpezarAsync(ModoApp.Trabajo);

        // Muchas veces seguidas, para agotar el resto de las palabras del modo.
        for (var i = 0; i < 100 && await repaso.ContarSinEmpezarAsync(ModoApp.Trabajo) > 0; i++)
        {
            await repaso.AgregarNuevasAsync(modo: ModoApp.Trabajo);
        }

        Assert.Equal(0, _base.Db.Palabras.Count(p => p.Tipo == TipoPalabra.Gramatica && p.Progreso != null));
        Assert.Equal(_base.Db.Palabras.Count(p => p.Tipo != TipoPalabra.Gramatica && p.Progreso != null), sinEmpezar);
        Assert.DoesNotContain(await repaso.CategoriasConNuevasAsync(ModoApp.Trabajo),
            c => TemasGramatica.EsCategoriaDeGramatica(c.Nombre));
    }

    [Fact]
    public async Task Practicar_un_tema_mete_todas_sus_frases_al_repaso_una_sola_vez()
    {
        var tema = TemasGramatica.Buscar("for-since")!;

        Assert.Equal(tema.Frases.Length, await _gramatica.PracticarAsync("for-since"));
        Assert.Equal(0, await _gramatica.PracticarAsync("for-since"));
        Assert.Equal(0, await _gramatica.PracticarAsync("no-existe"));

        var estado = (await _gramatica.ObtenerAsync("for-since"))!;
        Assert.True(estado.Completo);
        Assert.Equal(tema.Frases.Length, estado.EnRepaso);
        // Solo entró ese tema.
        Assert.Equal(tema.Frases.Length, _base.Db.Progresos.Count());
        Assert.All(await _gramatica.ListarAsync(), t => Assert.Equal(t.Tema.Clave == "for-since", t.Empezado));

        // Y ya le tocan al repaso del modo Trabajo.
        var pendiente = await Repaso().SiguientePendienteAsync(ModoApp.Trabajo);
        Assert.Equal(TipoPalabra.Gramatica, _base.Db.Palabras.Single(p => p.Id == pendiente!.PalabraId).Tipo);
    }

    [Fact]
    public void Una_frase_de_gramatica_da_corregir_completar_y_dictado()
    {
        var palabra = Frase("I have been working here for two years.");
        var posibles = new GeneradorEjercicios().Posibles(palabra);

        Assert.Equal(new[] { TipoEjercicio.CorregirFrase, TipoEjercicio.CompletarFrase, TipoEjercicio.DictadoPalabra },
            posibles.Select(e => e.Tipo));
        var completar = posibles.Single(e => e.Tipo == TipoEjercicio.CompletarFrase);
        Assert.Equal("I have been working here ___ two years.", completar.FraseConHueco);
        Assert.Equal("for", completar.Respuesta);
        Assert.Equal(palabra.Ingles, posibles.Single(e => e.Tipo == TipoEjercicio.CorregirFrase).Respuesta);
    }

    [Fact]
    public async Task Corregir_la_frase_acepta_contracciones_y_explica_si_la_dejaste_igual()
    {
        var palabra = Frase("I'm going to deploy the fix this afternoon.");
        _base.Db.Progresos.Add(new ProgresoPalabra { PalabraId = palabra.Id, ProximoRepaso = DateTime.UtcNow });
        await _base.Db.SaveChangesAsync();
        var repaso = Repaso();

        var bien = await repaso.EvaluarSinGuardarAsync(palabra.Id, TipoEjercicio.CorregirFrase, "I am going to deploy the fix this afternoon");
        Assert.True(bien!.Acerto);

        var igual = await repaso.EvaluarSinGuardarAsync(palabra.Id, TipoEjercicio.CorregirFrase, palabra.FraseIncorrecta);
        Assert.False(igual!.Acerto);
        Assert.Equal(new[] { "Escribiste la frase igual que estaba: el error sigue ahí." }, igual.Explicaciones);
    }
}
