# Vocabulario de inglés

Aplicación web para aprender vocabulario de inglés (empezando por tecnología y trabajo).
Cada palabra tiene su escritura, su pronunciación y su audio; los verbos muestran presente,
pasado y futuro. **Leer en contexto** (modo Trabajo, en Practicar): pegas un texto real en inglés (documentación, un README, un
correo), tocas las palabras que no conoces, escribes qué significan y entran a tu repaso con la frase donde
aparecieron como ejemplo. Los textos quedan en "Mis lecturas" y las palabras que ya aprendes salen resaltadas.
Si la palabra ya existe en la app, su significado aparece solo y no se duplica.

Cuando fallas, el resultado explica **por qué** (sin IA, con reglas): si escribiste otro tiempo verbal
("escribiste el presente; aquí va el pasado"), te faltó o sobró una letra (y cuál), escribiste otra palabra de la
app, respondiste en español o, en frases, qué palabras faltaron y cuáles sobraron (`Services/ExplicadorErrores.cs`).

La app tiene **3 modos**, que eliges al entrar y se recuerdan (puedes cambiar desde el Inicio):

- **Principiante:** el vocabulario general de Pre A1 a A2.
- **Trabajo:** el inglés técnico y las frases de trabajo y entrevista (A2 a B2), con Leer en contexto, Gramática, Situaciones
  guiadas, Hablar, Shadowing, el simulacro y Mis respuestas.
- **Niños:** el modo infantil.

Principiante y Trabajo tienen cada uno su repaso, sus palabras nuevas y sus errores; el Inicio avisa si hay
pendientes en el otro modo. La meta diaria, la racha y el progreso son uno solo.

El contenido va **por niveles del MCER** (el mismo orden que usa Cambridge), desde cero hasta intermedio alto:

| Nivel | Contenido |
|---|---|
| Pre A1 · Desde cero | Colores, números, animales, familia, el cuerpo |
| A1 · Principiante | Comida y bebida, la casa, la ropa, días y meses, verbos básicos |
| A2 · Básico | Tecnología, trabajo, verbos de programación, la ciudad, viajes, salud, verbos comunes |
| B1 · Intermedio | Stand-up, code review, entrevista, Git y GitHub, Agile y Scrum, reuniones y correo |
| B2 · Intermedio alto | Entrevista técnica |

"Palabras nuevas" en orden va de nivel en nivel (Pre A1 primero); también puedes elegir una categoría. Las listas
son propias, ordenadas según el MCER: **no** copian las listas oficiales de Cambridge, que tienen derechos de autor.

También hay frases para el trabajo y las entrevistas: stand-up, code review,
entrevista (incluida una respuesta de ejemplo a "Tell me about yourself"), Git y GitHub, Agile y Scrum,
entrevista técnica de .NET, y reuniones y correo. En el repaso siempre **escribes** la respuesta, y las palabras que fallas vuelven
antes. Cada vez se elige al azar uno de estos ejercicios:

- Escribir la palabra en inglés a partir del español.
- Completar una frase (para los verbos, también en pasado y en futuro: "Yesterday I ___ a bug").
- Dictado: oyes la palabra o la frase de ejemplo y la escribes (no cuentan puntos ni mayúsculas).

Con las frases de trabajo hay tres ejercicios: escribir la frase completa a partir del español,
completar su palabra clave ("I'm ___ on the login page") y el dictado de la frase. Las contracciones
cuentan igual que la forma larga ("I'm" = "I am", "don't" = "do not").

**Shadowing** (en Practicar): por frase, 3 pasos. 1) Escuchas. 2) Repites **al mismo tiempo** que el audio,
copiando ritmo y entonación (lento y normal, las veces que quieras). 3) La dices tú solo y se marcan en verde y rojo
las palabras que se entendieron. Solo se califica el paso 3, porque mientras suena el audio el micrófono también
lo captaría; antes de grabar, la app detiene su audio. Es solo práctica: no se guarda nada.

**Gramática** (modo Trabajo, en Practicar): 11 temas que más usa un developer (presente, pasado y futuro, present perfect, for y since, will o
going to, preguntas, in/on/at, a/an/the, pasiva, condicional…), numerados en orden de estudio (primero A2, luego B1;
"Sigue este" marca el siguiente), con su regla corta en español y ejemplos de la frase
con el error típico de hispanohablantes junto a la correcta. Con "Practicar este tema" sus frases entran a tu repaso:
"Corrige la frase" (ves la frase con el error y la escribes bien), completar la parte gramatical y dictado. Las frases
de gramática no entran solas con las palabras nuevas del día. Los temas están en `Data/TemasGramatica.cs`.

**Tu ruta** (en el Inicio): 5 etapas de cero a intermedio alto (Pre A1 a B2), cada una con metas de vocabulario,
gramática, escuchar, hablar y leer. Marca tu etapa y "lo siguiente" sin bloquear nada; el avance se calcula solo con
lo que ya haces y el diagnóstico de nivel marca como hechas las etapas que ya dominas.

**Diagnóstico de nivel** (en Progreso y en Practicar): 23 preguntas de fácil a difícil (vocabulario, un tema de
gramática por pregunta y dictados) que estiman tu nivel en cada área, te dan tus 3 puntos débiles con enlace para
practicarlos y te dicen qué contenido le falta a la app si ya llegas a lo más difícil que tiene. Se guarda para
comparar tu avance (hazlo una vez al mes) y no cuenta para tu repaso. Si nunca lo has hecho, es lo primero que
recomienda Practicar.

**Sonidos difíciles** (modo Trabajo, en Practicar → Hablar): los 10 sonidos del inglés que el español no tiene
(ship/sheep, th, v/b, h, z, -ed…), en orden, con "cómo se hace" en español y pares mínimos para escuchar. "Escucha y
elige" entrena el oído y "Dila tú" usa el reconocimiento de voz para ver qué palabra se entendió. Un sonido se domina
con 8 de tus últimas 10 respuestas.

**Situaciones guiadas** (modo Trabajo, en Practicar): diálogos cortos de daily stand-up, entrevista, code review y
pedir ayuda con un bug. La app dice su parte en voz alta y tú respondes con una de las respuestas modelo (con su
traducción). Cuenta como bien si se entiende al menos el 80 % de las palabras de alguna de ellas. Al final dice
cuántos turnos respondiste bien a la primera. Los diálogos están en `Data/SituacionesGuiadas.cs`. Es solo práctica:
no se guarda nada.

**Modo niños** (en Practicar): para preescolar, con dibujos grandes, audio y tocar la respuesta (sin escribir).

- 4 categorías de Pre A1: Colores, Números, Animales y El cuerpo (48 palabras). Los dibujos son SVG propios
  (`Data/DibujosInfantiles.cs`): colores en círculos, números con puntitos para contar, caritas de animales y
  una figura con la parte del cuerpo resaltada.
- **Aprende:** tarjetas que dicen la palabra al tocarlas. **Escucha y toca:** 8 rondas; la app dice la palabra y
  el niño elige entre 3 dibujos de la misma categoría; al final, de 1 a 3 estrellas.
- Perfil del niño (nombre opcional) con su propio progreso (`ProgresosInfantiles`), aparte del tuyo: una palabra
  queda aprendida con 3 aciertos seguidos y el juego repite primero las que falló.

**Practicar en voz alta** (menú "Hablar"): eliges una categoría, escuchas una frase de trabajo o de
entrevista, la dices y el navegador marca en verde las palabras que entendió y en rojo las que no.
Es solo práctica: no cuenta para el repaso ni para la meta, y no se guarda nada.

- Usa el reconocimiento de voz del navegador (Web Speech API): gratis y sin claves. Funciona en Chrome
  y Edge (computadora y Android); en Firefox no está disponible y la página lo avisa.
- El navegador puede enviar el audio a su propio servicio de reconocimiento (en Chrome, de Google), así
  que necesita internet.
- El micrófono solo funciona en `localhost` o con **https**. Si abres la app desde el celular con la IP de
  tu computadora (`http://192.168...`), el navegador no deja usar el micrófono.

**Simulacro de entrevista** (menú "Entrevista"): 5 preguntas típicas, empezando siempre por
"Tell me about yourself". Escuchas cada una, la respondes en voz alta o por escrito y luego ves una
respuesta modelo con un consejo. Las preguntas están en `Data/PreguntasEntrevista.cs` (escritas a mano).
Es solo práctica: no se guarda nada.

**Mis respuestas** (enlace en el simulacro): escribes en inglés tu propia respuesta a cada una de las 10
preguntas, con tus datos reales. En el simulacro ves tu respuesta primero (con 🔊) y el ejemplo genérico
queda como "Otro ejemplo"; si respondiste en voz alta o por escrito, se marcan en verde las palabras de tu
respuesta que dijiste y en rojo las que faltaron. Se guardan en tu base de datos (tabla `RespuestasPropias`),
no en el repositorio. Cada pregunta tiene una clave fija (`Clave`): no la cambies aunque corrijas el texto
de la pregunta, o se pierde la relación con tu respuesta.

**Progreso** (menú "Progreso"): días seguidos cumpliendo tu meta (si hoy todavía no la cumples, la racha
cuenta hasta ayer), tu mejor racha, los ejercicios por día de las últimas 2 semanas y las 10 palabras que más
fallas. La racha usa tu meta actual también para los días anteriores. Desde ahí puedes:

- **Practicar mis errores:** repasar ahora las palabras que más fallas. Es práctica extra: no cambia cuándo
  te toca repasarlas ni cuenta para tu meta. También está en la pantalla de Repaso.
- **Descargar respaldo:** baja una copia de `vocabulario.db`. Para restaurarla, cierra la app y reemplaza
  `vocabulario.db` con la copia. La app no tiene usuarios ni contraseñas: si algún día la publicas en
  internet, cualquiera podría descargar ese archivo.

La app se ajusta a ti:

- **Sin repeticiones de más:** si aciertas una palabra, ya no vuelve el mismo día (pasa a mañana, luego a 3, 7, 14
  y 30 días). Si la fallas, vuelve al final de la sesión. Cuando ya no quedan pendientes, la sesión termina.
- **Más repetición de lo que fallas:** si fallaste 2 o más de las últimas 5 respuestas de una palabra,
  sus intervalos se reducen a la mitad (1, 1, 3, 7 y 15 días) hasta que la aciertes varias veces seguidas.
- **Palabras nuevas a tu ritmo:** el botón calcula cuántas te tocan según tus aciertos en las últimas
  50 respuestas: 90% o más → 8, 75–89% → 5, 60–74% → 3 y menos de 60% → 0 (primero refuerza).
  Con menos de 10 respuestas da 5. Puedes elegir de qué categoría salen (por ejemplo, solo
  "Entrevista"); la app recuerda tu última elección.
- **Meta diaria:** eliges cuántos ejercicios quieres responder al día (en "Mi meta") y ves tu avance
  en Inicio y en Repaso. "Hoy" se cuenta con la hora de México (`ZonaHoraria` en `appsettings.json`).

El plan de cambios futuros, por fases, está en [PLAN.md](PLAN.md).

## Tecnologías

- ASP.NET Core 10 (MVC) y Razor
- Estilos propios en `wwwroot/css/site.css`, sin Bootstrap ni archivos de internet: la app carga igual sin
  conexión (salvo el reconocimiento de voz). Diseño para celular primero: barra inferior con 4 pestañas
  (Inicio, Repaso, Practicar, Progreso), una acción principal por pantalla y botones de al menos 44px.
  El repaso va en "modo enfocado" (sin pestañas) con "Comprobar" y el resultado abajo, al alcance del pulgar.
  Los íconos están en `Views/Shared/_Icono.cshtml`.
- Entity Framework Core con SQLite (un solo archivo, `vocabulario.db`, que se crea solo) y migraciones
- Patrón **Strategy** para el algoritmo de repaso (`IAlgoritmoRepaso`): hoy `IntervalosAdaptativos`,
  que usa los intervalos fijos (1, 3, 7, 14 y 30 días; un fallo vuelve en la misma sesión) y los acorta para las palabras difíciles;
  se puede cambiar a `IntervalosFijos` o a FSRS en `Program.cs` sin tocar el resto
- Patrón **Strategy** también para el ritmo de palabras nuevas (`IRitmoPalabrasNuevas`, hoy `RitmoPorAciertos`)
- Audio con la voz del navegador (Web Speech API) y manifiesto para instalarla en el celular

## Cómo correrla

1. Instalar el SDK de .NET 10.
2. En esta carpeta: `dotnet run`
3. Abrir http://localhost:5080

## Pruebas

Las pruebas automáticas (xUnit) están en `Pruebas/`, un proyecto aparte que la app no compila:

```
dotnet test Pruebas
```

Cubren las reglas del repaso (intervalos fijos y adaptativos), el ritmo de palabras nuevas, la comparación
de respuestas (contracciones, puntuación), el generador de ejercicios y `RepasoService` contra una base
SQLite en memoria creada con las migraciones reales.

GitHub las corre solas en cada pull request y en cada push a `main`
(`.github/workflows/pruebas.yml`); el resultado aparece como ✅ o ❌ en el pull request.

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
- `Data/`: AppDbContext, migraciones, el preparador de la base, las palabras iniciales (`SeedData.cs`)
  y las frases de trabajo (`FrasesTrabajo.cs`)
- `Services/`: algoritmos de repaso, ritmo de palabras nuevas, generador de ejercicios, comparación de
  respuestas (`ComparadorRespuestas`) y `RepasoService`
- `Pruebas/`: pruebas automáticas
- `Controllers/` y `Views/`: pantallas de inicio, lista de palabras y repaso

## Datos y licencias

- Pronunciación (IPA, inglés de Estados Unidos): [open-dict-data/ipa-dict](https://github.com/open-dict-data/ipa-dict),
  licencia MIT (derivado de cmudict-ipa). No se usa el archivo de inglés británico porque su licencia es GPL 3.0.
- Formas de los verbos: [monolithpl/verb.forms.dictionary](https://github.com/monolithpl/verb.forms.dictionary),
  licencia MIT (Copyright 2016 Wiktor Jakubczyc).
- La columna "Se lee" (aproximación en español) está escrita a mano y debe revisarse.
- El vocabulario general de Pre A1 a A2 (`Data/VocabularioGeneral.cs`, 168 palabras) tiene traducciones, "se lee" y
  ejemplos escritos a mano (por revisar); su IPA y las formas de sus verbos se tomaron de los dos conjuntos de datos
  MIT de arriba.
- Las frases en pasado y futuro de los verbos (`Data/FrasesVerbos.cs`) están escritas a mano y deben revisarse.
- Las frases de trabajo y entrevista (`Data/FrasesTrabajo.cs`) están escritas a mano y deben revisarse.
  Las de "Tell me about yourself" son un ejemplo: cambia los años, las tecnologías y el proyecto por los tuyos.
  Las categorías nuevas que agregues ahí se cargan solas en una base que ya existe, sin tocar tu progreso.

## Pendiente

- Revisar traducciones, ejemplos y la columna "Se lee".
- Más categorías y, después, otras profesiones.
