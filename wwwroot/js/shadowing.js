// Shadowing en 3 pasos: 1) escuchar, 2) repetir al mismo tiempo que el audio (lento y normal),
// 3) decirla solo; únicamente el paso 3 se califica (con la misma comparación tolerante de "Hablar").
(function () {
    var tarjeta = document.getElementById('shadowing');
    if (!tarjeta) {
        return;
    }

    var frase = tarjeta.getAttribute('data-frase');
    var estado = document.getElementById('estado');
    var botonDecir = document.getElementById('btn-decir');

    function marcarHecho(numero) {
        var paso = document.getElementById('paso-' + numero);
        if (paso) {
            paso.classList.add('hecho');
        }
    }

    // Pasos 1 y 2: reproducir la frase (normal o lento).
    document.querySelectorAll('.btn-paso').forEach(function (boton) {
        boton.addEventListener('click', function () {
            if (typeof hablar === 'function' && 'speechSynthesis' in window) {
                hablar(frase, boton.getAttribute('data-velocidad') === 'lento');
            }
            marcarHecho(boton.closest('.paso').getAttribute('data-paso'));
        });
    });

    // Paso 3: decirla solo y calificar.
    var Reconocedor = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!Reconocedor) {
        document.getElementById('sin-soporte').hidden = false;
        botonDecir.disabled = true;
        return;
    }

    function mostrar(resultado) {
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
        document.getElementById('puntaje').textContent = resultado.aciertos === resultado.total
            ? '¡Perfecto! Se entendieron las ' + resultado.total + ' palabras.'
            : 'Se entendieron ' + resultado.aciertos + ' de ' + resultado.total + ' palabras. Vuelve al paso 2 con las rojas.';
        document.getElementById('entendido').textContent = resultado.entendido;
        document.getElementById('resultado').hidden = false;
        marcarHecho(3);
    }

    botonDecir.addEventListener('click', function () {
        // El audio de la app no debe sonar mientras grabas: se detiene antes de escuchar.
        if ('speechSynthesis' in window) {
            window.speechSynthesis.cancel();
        }

        var reconocimiento = new Reconocedor();
        reconocimiento.lang = 'en-US';
        reconocimiento.interimResults = false;
        reconocimiento.maxAlternatives = 3;
        var huboResultado = false;
        var huboError = false;

        reconocimiento.onstart = function () {
            botonDecir.disabled = true;
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
                'not-allowed': 'Necesito permiso para usar el micrófono.',
                'service-not-allowed': 'Necesito permiso para usar el micrófono.',
                'no-speech': 'No te escuché. Intenta otra vez, un poco más fuerte.',
                'audio-capture': 'No encontré un micrófono.',
                'network': 'El reconocimiento de voz necesita internet.'
            };
            estado.textContent = mensajes[evento.error] || 'No se pudo reconocer la voz (' + evento.error + ').';
        };
        reconocimiento.onend = function () {
            botonDecir.disabled = false;
            if (!huboResultado && !huboError) {
                estado.textContent = 'No te escuché. Intenta otra vez.';
            }
        };
        reconocimiento.start();
    });
})();
