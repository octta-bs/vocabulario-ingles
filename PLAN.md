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

## Fase 5 · Funciones inspiradas en otras apps 🔄

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
- [ ] **Situaciones guiadas** (como el roleplay de Duolingo y Speak): diálogos armados de stand-up y de entrevista;
  la app dice su parte y tú respondes en voz alta.

Descartadas por ahora:

- Conversación libre con IA: necesita un servicio de pago y la regla es no gastar dinero.
- Pronunciación sonido por sonido (como ELSA): necesita modelos especializados que el navegador no tiene.
- Cambiar el algoritmo de repaso a FSRS (como Anki): vale la pena, pero cuando haya semanas de historial para
  notar la diferencia. El patrón Strategy (`IAlgoritmoRepaso`) ya permite cambiarlo sin tocar lo demás.

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
