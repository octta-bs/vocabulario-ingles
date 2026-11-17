# Vocabulario de inglés

Aplicación web para aprender vocabulario de inglés (empezando por tecnología y trabajo).
Cada palabra tiene su escritura, su pronunciación y su audio; los verbos muestran presente,
pasado y futuro. En el repaso siempre **escribes** la respuesta, y las palabras que fallas vuelven
antes. Cada vez se elige al azar uno de estos ejercicios:

- Escribir la palabra en inglés a partir del español.
- Completar una frase (para los verbos, también en pasado y en futuro: "Yesterday I ___ a bug").
- Dictado: oyes la palabra o la frase de ejemplo y la escribes (no cuentan puntos ni mayúsculas).

## Tecnologías

- ASP.NET Core 10 (MVC) y Razor
- Entity Framework Core con SQLite (un solo archivo, `vocabulario.db`, que se crea solo)
- Patrón **Strategy** para el algoritmo de repaso (`IAlgoritmoRepaso`): hoy intervalos fijos
  (mismo día, 1, 3, 7, 14 y 30 días); se puede cambiar a FSRS sin tocar el resto
- Audio con la voz del navegador (Web Speech API) y manifiesto para instalarla en el celular

## Cómo correrla

1. Instalar el SDK de .NET 10.
2. En esta carpeta: `dotnet run`
3. Abrir http://localhost:5080

## Estructura

- `Models/`: Palabra, Categoria, ProgresoPalabra
- `Data/`: AppDbContext y las palabras iniciales (`SeedData.cs`)
- `Services/`: algoritmo de repaso y `RepasoService`
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
- Cambiar `EnsureCreated` por migraciones de Entity Framework cuando el modelo empiece a cambiar.
- Pruebas automáticas del algoritmo de repaso.
- Más categorías y, después, otras profesiones.
