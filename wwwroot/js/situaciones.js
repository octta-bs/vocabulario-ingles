// Situaciones guiadas: la app dice su parte (voz del navegador) y tú respondes en voz alta con una de las
// respuestas modelo. Se califica con la misma comparación tolerante de "Hablar" (Practica.mejor).
// Es solo práctica: no se guarda nada.
var Situaciones = (function () {
    // Si se entiende al menos el 80 % de las palabras de una respuesta modelo, el turno cuenta como bien.
    var MINIMO = 0.8;

    // Compara lo que se entendió con cada respuesta modelo y se queda con la que más se parece.
    function calificar(respuestas, alternativas) {
        var elegido = null;
        respuestas.forEach(function (respuesta, indice) {
            var r = Practica.mejor(respuesta, alternativas);
            r.indice = indice;
            r.proporcion = r.total ? r.aciertos / r.total : 0;
            if (!elegido || r.proporcion > elegido.proporcion) {
                elegido = r;
            }
        });
        elegido.bien = elegido.proporcion >= MINIMO;
        return elegido;
    }

    return { calificar: calificar, MINIMO: MINIMO };
})();

(function () {
    var datos = document.getElementById('turnos');
    if (!datos) {
        return;
    }

    var turnos = JSON.parse(datos.textContent);
    var dialogo = document.getElementById('dialogo');
    var panel = document.getElementById('turno');
    var estado = document.getElementById('estado');
    var botonResponder = document.getElementById('btn-responder');
    var botonSiguiente = document.getElementById('btn-siguiente');
    var Reconocedor = window.SpeechRecognition || window.webkitSpeechRecognition;

    var actual = 0;
    var bienALaPrimera = 0;
    var intentos = 0;
    var ultimo = null; // el mejor resultado del turno actual

    function leer(texto) {
        if (typeof hablar === 'function' && 'speechSynthesis' in window) {
            hablar(texto, false);
        }
    }

    function burbuja(quien, ingles, espanol) {
        var li = document.createElement('li');
        li.className = 'burbuja burbuja-' + quien;
        var texto = document.createElement('p');
        texto.className = 'mb-0';
        texto.textContent = ingles;
        li.appendChild(texto);
        if (espanol) {
            var detalle = document.createElement('details');
            var resumen = document.createElement('summary');
            resumen.textContent = 'Ver en español';
            detalle.appendChild(resumen);
            detalle.appendChild(document.createTextNode(espanol));
            li.appendChild(detalle);
        }
        dialogo.appendChild(li);
    }

    function mostrarTurno() {
        var turno = turnos[actual];
        intentos = 0;
        ultimo = null;
        burbuja('app', turno.dice, turno.diceEspanol);
        leer(turno.dice);

        document.getElementById('numero-turno').textContent = 'Turno ' + (actual + 1) + ' de ' + turnos.length;
        var lista = document.getElementById('respuestas');
        lista.textContent = '';
        turno.respuestas.forEach(function (r) {
            var li = document.createElement('li');
            var boton = document.createElement('button');
            boton.type = 'button';
            boton.className = 'btn btn-sm btn-outline-secondary btn-audio';
            boton.setAttribute('data-texto', r.ingles);
            boton.setAttribute('aria-label', 'Escuchar: ' + r.ingles);
            boton.textContent = '🔊';
            var textos = document.createElement('span');
            var ingles = document.createElement('span');
            ingles.className = 'ingles';
            ingles.textContent = r.ingles;
            var espanol = document.createElement('span');
            espanol.className = 'espanol';
            espanol.textContent = r.espanol;
            textos.appendChild(ingles);
            textos.appendChild(espanol);
            li.appendChild(boton);
            li.appendChild(textos);
            lista.appendChild(li);
        });

        document.getElementById('resultado').hidden = true;
        estado.textContent = '';
        botonSiguiente.hidden = !!Reconocedor;
        botonSiguiente.textContent = Reconocedor ? 'Siguiente' : 'Ya la dije';
        panel.hidden = false;
        panel.scrollIntoView({ block: 'nearest' });
    }

    function mostrarResultado(resultado) {
        var contenedor = document.getElementById('palabras');
        contenedor.textContent = '';
        resultado.palabras.forEach(function (p, i) {
            if (i > 0) {
                contenedor.appendChild(document.createTextNode(' '));
            }
            var span = document.createElement('span');
            span.className = p.ok ? 'practica-ok' : 'practica-falta';
            span.textContent = p.texto;
            contenedor.appendChild(span);
        });
        document.getElementById('puntaje').textContent = resultado.bien
            ? (resultado.aciertos === resultado.total ? '¡Perfecto! 🎉' : '¡Bien! Se entendió tu respuesta.')
            : 'Se entendieron ' + resultado.aciertos + ' de ' + resultado.total + ' palabras. Inténtalo otra vez.';
        document.getElementById('entendido').textContent = resultado.entendido;
        document.getElementById('resultado').hidden = false;

        botonSiguiente.hidden = false;
        botonSiguiente.textContent = resultado.bien ? 'Siguiente' : 'Seguir sin repetir';
        botonSiguiente.scrollIntoView({ block: 'nearest' });
    }

    function siguiente() {
        var turno = turnos[actual];
        // Tu burbuja muestra la respuesta modelo que dijiste (o la primera, si no hubo calificación).
        var tuya = turno.respuestas[ultimo ? ultimo.indice : 0];
        burbuja('tu', tuya.ingles, null);
        if (ultimo && ultimo.bien && intentos === 1) {
            bienALaPrimera++;
        }

        actual++;
        if (actual < turnos.length) {
            mostrarTurno();
            return;
        }

        panel.hidden = true;
        document.getElementById('resumen').textContent = Reconocedor
            ? 'Respondiste bien a la primera ' + bienALaPrimera + ' de ' + turnos.length + ' turnos.'
            : 'Practicaste los ' + turnos.length + ' turnos.';
        var final = document.getElementById('final');
        final.hidden = false;
        final.scrollIntoView({ block: 'nearest' });
    }

    document.getElementById('btn-empezar').addEventListener('click', function () {
        document.getElementById('inicio-situacion').hidden = true;
        mostrarTurno();
    });

    document.getElementById('btn-repetir').addEventListener('click', function () {
        leer(turnos[actual].dice);
    });

    botonSiguiente.addEventListener('click', siguiente);

    if (!Reconocedor) {
        document.getElementById('sin-soporte').hidden = false;
        botonResponder.hidden = true;
        return;
    }

    botonResponder.addEventListener('click', function () {
        var reconocimiento = new Reconocedor();
        reconocimiento.lang = 'en-US';
        reconocimiento.interimResults = false;
        reconocimiento.maxAlternatives = 3;
        var huboResultado = false;
        var huboError = false;

        reconocimiento.onstart = function () {
            botonResponder.disabled = true;
            document.getElementById('resultado').hidden = true;
            estado.textContent = 'Escuchando… di tu respuesta.';
        };

        reconocimiento.onresult = function (evento) {
            huboResultado = true;
            var alternativas = [];
            for (var i = 0; i < evento.results[0].length; i++) {
                alternativas.push(evento.results[0][i].transcript);
            }
            estado.textContent = '';
            intentos++;
            var respuestas = turnos[actual].respuestas.map(function (r) { return r.ingles; });
            var resultado = Situaciones.calificar(respuestas, alternativas);
            // Se guarda el mejor intento del turno, para la burbuja y el resumen.
            if (!ultimo || resultado.bien || resultado.proporcion > ultimo.proporcion) {
                ultimo = resultado;
            }
            mostrarResultado(resultado);
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
            botonResponder.disabled = false;
            if (!huboResultado && !huboError) {
                estado.textContent = 'No te escuché. Intenta otra vez.';
            }
        };

        window.speechSynthesis && window.speechSynthesis.cancel();
        reconocimiento.start();
    });
})();
