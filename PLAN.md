# Plan de la app

Para que la app crezca en orden: cada fase se termina **y se usa** antes de empezar la siguiente.
Las ideas nuevas se anotan en "Ideas para después" y se acomodan en una fase; no se programan sobre la marcha.

Estado: ⬜ pendiente · 🔄 en curso · ✅ hecha

## Lo que ya existe (hasta noviembre de 2026)

Repaso escrito con intervalos adaptativos, meta diaria, ritmo de palabras nuevas por aciertos, niveles del MCER
(Pre A1 a B2), frases de trabajo y entrevista, Hablar, Shadowing, simulacro de entrevista con "Mis respuestas",
Progreso (racha, gráfica, errores), práctica de errores, respaldo, rediseño móvil sin Bootstrap, modo niños,
pruebas automáticas y GitHub Actions. Detalle en el [README](README.md).

## Fase 1 · Ordenar en modos ✅

Que la app se separe en 3 modos y cada uno muestre solo lo suyo.

- **Principiante:** palabras generales de Pre A1 a A2 (colores, comida, casa, ciudad, viajes, salud…).
  Repaso y palabras nuevas, sin pantallas de trabajo.
- **Trabajo:** inglés técnico y frases de A2 a B2. Repaso y palabras nuevas de esas categorías, más Hablar,
  Shadowing, simulacro de entrevista, Mis respuestas y Mis errores.
- **Niños:** el modo infantil que ya existe.
- Al entrar eliges tu modo; la app lo recuerda y el Inicio muestra en cuál estás, con un botón para cambiar.

Decisión tomada:

- [x] El repaso **se separa por modo**: cada modo repasa solo sus palabras; el Inicio avisa cuántas pendientes
  hay en el otro modo. La meta diaria cuenta los dos.

Listo cuando: los 3 modos funcionan, cada pantalla aparece solo en su modo, el progreso guardado no cambia y las
pruebas automáticas y en el navegador pasan.

Hecho: el modo se elige al entrar y se recuerda en el navegador (cookie, sin cambios en la base). Principiante son
las categorías de `Data/VocabularioGeneral.cs`; Trabajo, todas las demás. Por modo se separan el repaso, las palabras
nuevas, Mis errores, Practicar y la lista de palabras; la meta, la racha y el progreso son uno solo.

## Fase 2 · Usarla una semana 🔄

Sin programar. Seguir la rutina diaria (repaso 10 min, shadowing 5 min, simulacro un día sí y uno no) y anotar:

- [ ] Qué aburre, qué cuesta o qué estorba al usarla.
- [ ] Qué palabras y frases se fallan más (pantalla Progreso).
- [ ] Si el audio en inglés y el micrófono funcionan en el celular.
- [ ] Si el modo niños funciona con un niño de verdad (audio y dibujos).

Listo cuando: hay una lista de ajustes concretos, que se hacen antes de la Fase 3.

## Fase 3 · Usarla desde el celular ⬜

Hoy el micrófono solo funciona en `localhost`; en el celular necesita https.

- [ ] Elegir dónde publicarla (de preferencia gratis) y con https.
- [ ] Inicio de sesión, para que nadie más vea el progreso, "Mis respuestas" ni el respaldo.
- [ ] Pasar el progreso actual (`vocabulario.db`) a la versión publicada.

Listo cuando: se usa desde el celular con micrófono y los datos están protegidos.

## Fase 4 · Más contenido según lo que se falle ⬜

Con los datos de Progreso, no a ciegas. Candidatos:

- [ ] Vocabulario general de B1 y B2.
- [ ] Más preguntas de entrevista técnica.
- [ ] Más categorías para niños (comida, familia, casa).
- [ ] Revisión del inglés escrito a mano (frases, ejemplos y "se lee").

## Fase 5 · Funciones inspiradas en otras apps ✅

Salen de comparar la app con Anki, LingQ, Duolingo, ELSA Speak y Speak (noviembre de 2026). Todas son gratis
y sin IA de pago. Van después de la Fase 2, porque el uso real dirá cuál conviene primero. Orden recomendado:

- [x] **Leer en contexto** (como LingQ): pegar un texto real del trabajo (documentación, un README, un correo),
  tocar las palabras que no se conocen y meterlas al repaso con su frase. Es lo que más acerca al inglés real de
  un developer. Hecho: "Mis lecturas" (modo Trabajo, en Practicar) guarda los textos (tabla `Lecturas`); el
  significado lo escribes tú (o aparece solo si la palabra ya existe); la palabra entra directo al repaso en la
  categoría "Mis lecturas" (B1) con la frase como ejemplo. Sin IPA para estas palabras (el audio sí funciona).
- [x] **Explicar el error sin IA** (como "Explain My Answer" de Duolingo): en vez de solo mostrar la respuesta
  correcta, decir por qué con reglas sencillas: tiempo verbal equivocado ("usaste presente; aquí va pasado"),
  una letra de más o de menos, o una palabra parecida. Hecho en `Services/ExplicadorErrores.cs`: nada escrito,
  respuesta en español, tiempo verbal, plural, una letra (faltó, sobró, cambiada o volteada), otra palabra de la
  app y, en frases, qué faltó y qué sobró. Se muestra en el resultado del repaso y de Mis errores.
- [x] **Situaciones guiadas** (como el roleplay de Duolingo y Speak): diálogos armados de stand-up y de entrevista;
  la app dice su parte y tú respondes en voz alta. Hecho: 4 situaciones (daily stand-up, entrevista, code review y
  pedir ayuda con un bug) en `Data/SituacionesGuiadas.cs`, sin tocar la base. Cada turno trae de 1 a 3 respuestas
  modelo; cuenta como bien con el 80 % de las palabras de alguna, con la misma comparación de Hablar.

Descartadas por ahora:

- Conversación libre con IA: necesita un servicio de pago y la regla es no gastar dinero.
- Pronunciación sonido por sonido (como ELSA): necesita modelos especializados que el navegador no tiene.
- Cambiar el algoritmo de repaso a FSRS (como Anki): vale la pena, pero cuando haya semanas de historial para
  notar la diferencia. El patrón Strategy (`IAlgoritmoRepaso`) ya permite cambiarlo sin tocar lo demás.

## Fase 6 · Gramática ✅

Se adelantó antes de terminar la Fase 2, a pedido. Los temas que más usa un developer, enfocados en los errores
típicos de hispanohablantes.

- [x] 11 temas (presente, pasado y futuro; present perfect o pasado, for y since, will o going to, pedir con cortesía, orden de las preguntas,
  in/on/at, a/an/the, voz pasiva, primer condicional y errores típicos), cada uno con su regla corta en español y
  6 o 7 frases de trabajo con su versión con el error (`Data/TemasGramatica.cs`).
- [x] Tú eliges el tema: "Practicar este tema" mete sus frases a tu repaso del modo Trabajo. No entran solas con
  las palabras nuevas del día.
- [x] Ejercicios: "Corrige la frase" (nuevo), completar la parte gramatical y dictado. El resultado muestra la regla.

Ajustes tras usarla: los temas van numerados de fácil a avanzado (A2 y luego B1) con "Sigue este"; las situaciones
guiadas también van por dificultad; y en el repaso un acierto ya no se repite el mismo día.

Pendiente: ajustar los temas con lo que se falle en la Fase 2.

## Fase 7 · Diagnóstico de nivel 🔄

Un examen corto que dice tu nivel por área, tus puntos débiles y qué contenido le falta a la app. Así lo que se
agrega sale de tus resultados y no de adivinar (apoya a la Fase 4).

- [x] Paso 1: 23 preguntas de fácil a difícil (8 de vocabulario de Pre A1 a B2, 1 por cada tema de gramática y
  4 dictados de A1 a B2). Nivel por área: el más alto al que llegas acertando al menos la mitad de cada nivel, sin
  saltarte ninguno. Muestra tus 3 puntos débiles con enlace para practicarlos y "lo que le falta a la app" cuando
  llegas al nivel más alto de un área. Se guarda (tabla `Diagnosticos`) para compararlo cada mes; no toca el repaso.
- [ ] Paso 2: hablar (decir 2 o 3 frases con el micrófono).

## Fase 8 · Sonidos difíciles ✅

Fonética para hispanohablantes, sin IA de pago: los 10 sonidos del inglés de EE. UU. que el español no tiene
(sin "e" antes de s, v/b, h, i corta y larga, sh/ch, z, th sorda y sonora, a abierta y corta, -ed), en orden de
estudio, con pares mínimos (ship / sheep) en `Data/SonidosDificiles.cs`.

- [x] "Cómo se hace" en español y los pares para escuchar.
- [x] "Escucha y elige" (8 rondas con la voz del navegador) y "Dila tú" (el reconocimiento de voz dice qué palabra
  entendió; si no entiende ninguna de las dos, no cuenta).
- [x] Avance guardado (tabla `RespuestasSonido`): un sonido se domina con 8 de tus últimas 10 respuestas; "Sigue
  este" marca el primero sin dominar.

Límite conocido: no dice cómo mover la lengua (como ELSA); solo si se entendió la palabra que querías decir.

## Ruta de aprendizaje ✅

Un camino de cero a intermedio alto en 5 etapas (Pre A1, A1, A2, B1 y B2), cada una con metas de vocabulario,
gramática, escuchar, hablar y leer, armado con el contenido que ya existe (`Data/RutaAprendizaje.cs`).

- [x] Guía sin bloquear: marca tu etapa y "lo siguiente", pero todo se puede abrir cuando quieras.
- [x] El avance sale de lo que ya se guarda: palabras y frases aprendidas (acertadas en días distintos), temas de
  gramática, sonidos dominados, lecturas y respuestas escritas. Situaciones, shadowing y simulacro se sugieren sin
  medirse.
- [x] Una etapa se completa con el 80 % de sus metas medibles, o si el diagnóstico ya te pone en ese nivel.
- [x] En el Inicio: tu etapa, el porcentaje y "Lo siguiente".

## Revisión del contenido ✅

Revisión de todo el contenido (vocabulario, frases, gramática y sonidos) como la haría un profesor. Los cambios, el
porqué y las dudas de nivel para confirmar en el English Vocabulary Profile están en
[docs/revision-contenido.md](docs/revision-contenido.md).

- [ ] Confirmar en englishprofile.org (con tu cuenta) las 15 dudas de nivel de la revisión.
- [ ] Una conversación a la semana con un nativo (intercambio de idiomas).

## Ideas para después

Anotarlas aquí antes de programarlas:

- Varios perfiles de niño.
- Modo shadowing calificando mientras se habla (solo con audífonos).
- Algoritmo de repaso FSRS, cuando haya suficiente historial.

## Reglas de trabajo

- Cambios pequeños, uno a la vez; las decisiones importantes se preguntan antes.
- Probar de verdad (pruebas automáticas y en el navegador) antes de decir que algo está listo.
- Pedir permiso antes de cada commit y push.
- Nunca perder el progreso guardado: migraciones de EF con respaldo automático.
- Datos de pronunciación solo de inglés de EE. UU. con licencia MIT; nada con licencia GPL; nada de claves en el repositorio.
