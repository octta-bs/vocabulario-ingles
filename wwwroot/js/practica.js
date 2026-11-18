// Práctica en voz alta: el navegador reconoce lo que dices (Web Speech API, sin costo ni claves)
// y se marca cada palabra de la frase en verde si la entendió o en rojo si no.
// Es solo práctica: no se guarda nada.
var Practica = (function () {
    // "I'm" y "I am" cuentan igual (la misma lista que se usa al calificar el repaso).
    var contracciones = {
        "i'm": "i am", "you're": "you are", "we're": "we are", "they're": "they are",
        "it's": "it is", "that's": "that is", "what's": "what is", "there's": "there is",
        "let's": "let us", "i'll": "i will", "you'll": "you will", "we'll": "we will",
        "it'll": "it will", "i've": "i have", "we've": "we have", "i'd": "i would",
        "don't": "do not", "doesn't": "does not", "didn't": "did not", "isn't": "is not",
        "aren't": "are not", "wasn't": "was not", "weren't": "were not", "won't": "will not",
        "can't": "cannot", "couldn't": "could not", "shouldn't": "should not", "haven't": "have not"
    };

    // El reconocimiento a veces escribe números con cifras ("3" en vez de "three").
    var numeros = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten',
        'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen', 'twenty'];

    // Convierte un texto en palabras comparables: minúsculas, sin puntuación, contracciones expandidas.
    function tokens(texto) {
        var limpio = (texto || '').toLowerCase().replace(/[’‘]/g, "'").replace(/[.,;:!?¿¡"“”()]/g, ' ');
        var resultado = [];
        limpio.split(/\s+/).forEach(function (palabra) {
            palabra = palabra.replace(/^'+|'+$/g, '');
            if (!palabra) {
                return;
            }
            if (contracciones[palabra]) {
                resultado.push.apply(resultado, contracciones[palabra].split(' '));
            } else if (/^\d+$/.test(palabra) && +palabra < numeros.length) {
                resultado.push(numeros[+palabra]);
            } else {
                resultado.push(palabra);
            }
        });
        // "can not" y "cannot" cuentan igual.
        for (var i = 0; i < resultado.length - 1; i++) {
            if (resultado[i] === 'can' && resultado[i + 1] === 'not') {
                resultado.splice(i, 2, 'cannot');
            }
        }
        return resultado;
    }

    // Compara la frase con lo que se entendió. Cada palabra de la frase está bien si todas sus
    // partes aparecen en lo entendido (cada palabra entendida se usa una sola vez).
    function comparar(frase, entendido) {
        var disponibles = {};
        tokens(entendido).forEach(function (t) {
            disponibles[t] = (disponibles[t] || 0) + 1;
        });

        var palabras = [];
        var aciertos = 0;
        frase.split(/\s+/).forEach(function (original) {
            var partes = tokens(original);
            if (partes.length === 0) {
                return;
            }

            var necesarias = {};
            partes.forEach(function (t) { necesarias[t] = (necesarias[t] || 0) + 1; });
            var ok = Object.keys(necesarias).every(function (t) { return (disponibles[t] || 0) >= necesarias[t]; });
            if (ok) {
                partes.forEach(function (t) { disponibles[t]--; });
                aciertos++;
            }
            palabras.push({ texto: original, ok: ok });
        });

        return { palabras: palabras, aciertos: aciertos, total: palabras.length };
    }

    // De varias interpretaciones posibles, se queda con la que más palabras acierta.
    function mejor(frase, alternativas) {
        var elegido = null;
        alternativas.forEach(function (texto) {
            var r = comparar(frase, texto);
            r.entendido = texto;
            if (!elegido || r.aciertos > elegido.aciertos) {
                elegido = r;
            }
        });
        return elegido;
    }

    return { tokens: tokens, comparar: comparar, mejor: mejor };
})();

(function () {
    var tarjeta = document.getElementById('practica');
    var boton = document.getElementById('btn-hablar');
    if (!tarjeta || !boton) {
        return;
    }

    var Reconocedor = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!Reconocedor) {
        document.getElementById('sin-soporte').hidden = false;
        boton.disabled = true;
        return;
    }

    var frase = tarjeta.getAttribute('data-frase');
    var estado = document.getElementById('estado');

    function mostrar(resultado) {
        var contenedor = document.getElementById('palabras');
        contenedor.textContent = '';
        resultado.palabras.forEach(function (p, i) {
            if (i > 0) {
                contenedor.appendChild(document.createTextNode(' '));
            }
            var span = document.createElement('span');
            span.className = p.ok ? 'practica-ok' : 'practica-falta';
            span.title = p.ok ? 'Se entendió' : 'No se entendió';
            span.textContent = p.texto;
            contenedor.appendChild(span);
        });

        document.getElementById('puntaje').textContent = resultado.aciertos === resultado.total
            ? '¡Perfecto! Se entendieron las ' + resultado.total + ' palabras. 🎉'
            : 'Se entendieron ' + resultado.aciertos + ' de ' + resultado.total + ' palabras. Las rojas, inténtalas otra vez.';
        document.getElementById('entendido').textContent = resultado.entendido;
        document.getElementById('resultado').hidden = false;
    }

    boton.addEventListener('click', function () {
        var reconocimiento = new Reconocedor();
        reconocimiento.lang = 'en-US';
        reconocimiento.interimResults = false;
        reconocimiento.maxAlternatives = 3;
        var huboResultado = false;
        var huboError = false;

        reconocimiento.onstart = function () {
            boton.disabled = true;
            estado.textContent = 'Escuchando… di la frase.';
        };

        reconocimiento.onresult = function (evento) {
            huboResultado = true;
            var alternativas = [];
            for (var i = 0; i < evento.results[0].length; i++) {
                alternativas.push(evento.results[0][i].transcript);
            }
            estado.textContent = '';
            mostrar(Practica.mejor(frase, alternativas));
        };

        reconocimiento.onerror = function (evento) {
            huboError = true;
            var mensajes = {
                'not-allowed': 'Necesito permiso para usar el micrófono. Revisa el candado junto a la dirección.',
                'service-not-allowed': 'Necesito permiso para usar el micrófono. Revisa el candado junto a la dirección.',
                'no-speech': 'No te escuché. Intenta otra vez, un poco más fuerte.',
                'audio-capture': 'No encontré un micrófono.',
                'network': 'El reconocimiento de voz necesita internet.'
            };
            estado.textContent = mensajes[evento.error] || 'No se pudo reconocer la voz (' + evento.error + ').';
        };

        reconocimiento.onend = function () {
            boton.disabled = false;
            if (!huboResultado && !huboError) {
                estado.textContent = 'No te escuché. Intenta otra vez.';
            }
        };

        window.speechSynthesis && window.speechSynthesis.cancel();
        reconocimiento.start();
    });
})();
