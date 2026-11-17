# Vocabulario de inglés

Aplicación web para aprender vocabulario de inglés (empezando por tecnología y trabajo).
Cada palabra tiene su escritura, su pronunciación y su audio; los verbos muestran presente,
pasado y futuro. En el repaso siempre **escribes** la respuesta, y las palabras que fallas vuelven
antes. Cada vez se elige al azar uno de estos ejercicios:

- Escribir la palabra en inglés a partir del español.
- Completar una frase (para los verbos, también en pasado y en futuro: "Yesterday I ___ a bug").
- Dictado: oyes la palabra o la frase de ejemplo y la escribes (no cuentan puntos ni mayúsculas).

La app se ajusta a ti:

- **Más repetición de lo que fallas:** si fallaste 2 o más de las últimas 5 respuestas de una palabra,
  sus intervalos se reducen a la mitad (1, 1, 3, 7 y 15 días) hasta que la aciertes varias veces seguidas.
- **Palabras nuevas a tu ritmo:** el botón calcula cuántas te tocan según tus aciertos en las últimas
  50 respuestas: 90% o más → 8, 75–89% → 5, 60–74% → 3 y menos de 60% → 0 (primero refuerza).
  Con menos de 10 respuestas da 5.
- **Meta diaria:** eliges cuántos ejercicios quieres responder al día (en "Mi meta") y ves tu avance
  en Inicio y en Repaso. "Hoy" se cuenta con la hora de México (`ZonaHoraria` en `appsettings.json`).

## Tecnologías

- ASP.NET Core 10 (MVC) y Razor
- Entity Framework Core con SQLite (un solo archivo, `vocabulario.db`, que se crea solo) y migraciones
- Patrón **Strategy** para el algoritmo de repaso (`IAlgoritmoRepaso`): hoy `IntervalosAdaptativos`,
  que usa los intervalos fijos (mismo día, 1, 3, 7, 14 y 30 días) y los acorta para las palabras difíciles;
  se puede cambiar a `IntervalosFijos` o a FSRS en `Program.cs` sin tocar el resto
- Patrón **Strategy** también para el ritmo de palabras nuevas (`IRitmoPalabrasNuevas`, hoy `RitmoPorAciertos`)
- Audio con la voz del navegador (Web Speech API) y manifiesto para instalarla en el celular

## Cómo correrla

1. Instalar el SDK de .NET 10.
2. En esta carpeta: `dotnet run`
3. Abrir http://localhost:5080

## Base de datos y migraciones

La base se crea y se actualiza sola al arrancar (`Data/PreparadorBaseDatos.cs`):

1. Si hay migraciones pendientes, primero hace un respaldo: `vocabulario.respaldo-AAAAMMDD-HHMMSS.db`.
2. Si tu `vocabulario.db` se creó con la versión anterior (con `EnsureCreated`), marca la migración
   `InicioBase` como ya aplicada, porque esas tablas ya existen. Tu progreso no se toca.
3. Aplica las migraciones que falten (`Data/Migraciones/`).

Para cambiar el modelo más adelante:

```
dotnet tool install --global dotnet-ef
dotnet ef migrations add NombreDelCambio -o Data/Migraciones
```

## Estructura

- `Models/`: Palabra, Categoria, ProgresoPalabra, RegistroRespuesta (historial) y Configuracion (meta)
- `Data/`: AppDbContext, migraciones, el preparador de la base y las palabras iniciales (`SeedData.cs`)
- `Services/`: algoritmos de repaso, ritmo de palabras nuevas, generador de ejercicios y `RepasoService`
- `Controllers/` y `Views/`: pantallas de inicio, lista de palabras y repaso

## Datos y licencias

- Pronunciación (IPA, inglés de Estados Unidos): [open-dict-data/ipa-dict](https://github.com/open-dict-data/ipa-dict),
  licencia MIT (derivado de cmudict-ipa). No se usa el archivo de inglés británico porque su licencia es GPL 3.0.
- Formas de los verbos: [monolithpl/verb.forms.dictionary](https://github.com/monolithpl/verb.forms.dictionary),
  licencia MIT (Copyright 2016 Wiktor Jakubczyc).
- La columna "Se lee" (aproximación en español) está escrita a mano y debe revisarse.
- Las frases en pasado y futuro de los verbos (`Data/FrasesVerbos.cs`) están escritas a mano y deben revisarse.

## Pendiente

- Revisar traducciones, ejemplos y la columna "Se lee".
- Pruebas automáticas del algoritmo de repaso y del ritmo de palabras nuevas.
- Más categorías y, después, otras profesiones.
