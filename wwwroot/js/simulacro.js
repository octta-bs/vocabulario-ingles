// Simulacro de entrevista: muestra las preguntas una por una, deja responder en voz alta
// (reconocimiento de voz del navegador) o por escrito, y después enseña una respuesta modelo.
(function () {
    var preguntas = JSON.parse(document.getElementById('preguntas').textContent);
    var tarjeta = document.getElementById('simulacro');
    var caja = document.getElementById('respuesta');
    var botonDictar = document.getElementById('btn-dictar');
    var estado = document.getElementById('estado');
    var actual = 0;

    // Si el navegador no reconoce voz, este aviso se queda visible en cada pregunta.
    var avisoSinVoz = '';

    function leer(texto, lento) {
        if (typeof hablar === 'function' && 'speechSynthesis' in window) {
            hablar(texto, lento);
        }
    }

    function mostrar(indice) {
        var p = preguntas[indice];
        document.getElementById('numero').textContent = 'Pregunta ' + (indice + 1) + ' de ' + preguntas.length;
        document.getElementById('pregunta').textContent = p.ingles;
        document.getElementById('pregunta-es').textContent = p.espanol;
        document.getElementById('consejo').textContent = p.consejo;

        var lista = document.getElementById('modelo-frases');
        lista.textContent = '';
        p.modelo.forEach(function (frase) {
            var li = document.createElement('li');
            li.className = 'mb-1';
            var boton = document.createElement('button');
            boton.type = 'button';
            boton.className = 'btn btn-sm btn-outline-secondary btn-audio me-2';
            boton.setAttribute('data-texto', frase);
            boton.setAttribute('aria-label', 'Escuchar: ' + frase);
            boton.textContent = '🔊';
            li.appendChild(boton);
            li.appendChild(document.createTextNode(frase));
            lista.appendChild(li);
        });

        caja.value = '';
        estado.textContent = avisoSinVoz;
        document.getElementById('modelo').hidden = true;
        document.getElementById('btn-modelo').hidden = false;
        document.getElementById('btn-siguiente').textContent =
            indice === preguntas.length - 1 ? 'Terminar' : 'Siguiente pregunta →';
        tarjeta.hidden = false;
    }

    document.getElementById('btn-escuchar').addEventListener('click', function () {
        leer(preguntas[actual].ingles, false);
    });
    document.getElementById('btn-lento').addEventListener('click', function () {
        leer(preguntas[actual].ingles, true);
    });

    document.getElementById('btn-modelo').addEventListener('click', function () {
        detenerDictado();
        document.getElementById('modelo').hidden = false;
        this.hidden = true;
    });

    document.getElementById('btn-siguiente').addEventListener('click', function () {
        actual++;
        if (actual >= preguntas.length) {
            tarjeta.hidden = true;
            document.getElementById('final').hidden = false;
            return;
        }
        mostrar(actual);
        leer(preguntas[actual].ingles, false);
    });

    // Responder en voz alta: lo que dices se va escribiendo en la caja de respuesta.
    var Reconocedor = window.SpeechRecognition || window.webkitSpeechRecognition;
    var reconocimiento = null;

    function detenerDictado() {
        if (reconocimiento) {
            reconocimiento.stop();
        }
    }

    if (!Reconocedor) {
        botonDictar.hidden = true;
        avisoSinVoz = 'Tu navegador no reconoce voz: escribe tu respuesta (o usa Chrome o Edge para hablar).';
    } else {
        botonDictar.addEventListener('click', function () {
            if (reconocimiento) {
                detenerDictado();
                return;
            }

            reconocimiento = new Reconocedor();
            reconocimiento.lang = 'en-US';
            reconocimiento.continuous = true;
            reconocimiento.interimResults = false;
            var errorMostrado = false;

            reconocimiento.onstart = function () {
                botonDictar.textContent = '⏹ Detener';
                estado.textContent = 'Escuchando… responde en inglés.';
            };

            reconocimiento.onresult = function (evento) {
                for (var i = evento.resultIndex; i < evento.results.length; i++) {
                    if (evento.results[i].isFinal !== false) {
                        var texto = evento.results[i][0].transcript.trim();
                        caja.value = (caja.value ? caja.value.trim() + ' ' : '') + texto;
                    }
                }
            };

            reconocimiento.onerror = function (evento) {
                errorMostrado = true;
                var mensajes = {
                    'not-allowed': 'Necesito permiso para usar el micrófono.',
                    'service-not-allowed': 'Necesito permiso para usar el micrófono.',
                    'no-speech': 'No te escuché. Intenta otra vez.',
                    'audio-capture': 'No encontré un micrófono.',
                    'network': 'El reconocimiento de voz necesita internet.'
                };
                estado.textContent = mensajes[evento.error] || 'No se pudo reconocer la voz (' + evento.error + ').';
            };

            reconocimiento.onend = function () {
                reconocimiento = null;
                botonDictar.textContent = '🎤 Responder en voz alta';
                if (!errorMostrado) {
                    estado.textContent = caja.value ? 'Listo. Compárala con la respuesta modelo.' : '';
                }
            };

            if ('speechSynthesis' in window) {
                window.speechSynthesis.cancel();
            }
            reconocimiento.start();
        });
    }

    mostrar(0);
})();
