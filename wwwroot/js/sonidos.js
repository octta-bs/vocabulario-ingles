// Sonidos difíciles: "Escucha y elige" (la voz del navegador dice una palabra de un par y eliges cuál oíste) y
// "Dila tú" (el reconocimiento de voz dice qué palabra entendió). Cada respuesta se guarda para medir tu avance.
var Sonidos = (function () {
    function palabras(texto) {
        return (texto || '').toLowerCase().replace(/[’‘]/g, "'").split(/[^a-z']+/).filter(Boolean);
    }

    // De lo que entendió el navegador: 'bien' si oyó la palabra que querías decir, 'contra' si oyó la otra
    // del par, 'nada' si no oyó ninguna de las dos.
    function evaluar(objetivo, contra, alternativas) {
        var oidas = alternativas.map(palabras);
        if (oidas.some(function (p) { return p.indexOf(objetivo) >= 0; })) {
            return 'bien';
        }
        if (oidas.some(function (p) { return p.indexOf(contra) >= 0; })) {
            return 'contra';
        }
        return 'nada';
    }

    return { evaluar: evaluar };
})();

(function () {
    var contenedor = document.getElementById('sonido');
    if (!contenedor) {
        return;
    }

    var clave = contenedor.getAttribute('data-sonido');
    var urlResponder = contenedor.getAttribute('data-responder');
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    var pares = JSON.parse(document.getElementById('pares').textContent);
    var avance = document.getElementById('avance-sonido');
    var hayVoz = 'speechSynthesis' in window && typeof hablar === 'function';
    var Reconocedor = window.SpeechRecognition || window.webkitSpeechRecognition;
    var RONDAS = 8;

    function alAzar(lista) {
        return lista[Math.floor(Math.random() * lista.length)];
    }

    // Una palabra al azar y la otra de su par.
    function elegirPalabra() {
        var par = alAzar(pares);
        var lado = Math.random() < 0.5 ? 0 : 1;
        return { palabra: par[lado], contra: par[1 - lado], par: par };
    }

    function guardar(ejercicio, acerto) {
        var datos = new FormData();
        datos.append('sonido', clave);
        datos.append('ejercicio', ejercicio);
        datos.append('acerto', acerto ? 'true' : 'false');
        datos.append('__RequestVerificationToken', token);
        fetch(urlResponder, { method: 'POST', body: datos })
            .then(function (r) { return r.ok ? r.json() : null; })
            .then(function (estado) {
                if (!estado) {
                    return;
                }
                avance.textContent = estado.dominado
                    ? '¡Ya dominas este sonido! Puedes seguir practicando.'
                    : 'Tu avance: ' + estado.aciertos + ' de ' + estado.respondidas +
                      ' en tus últimas respuestas (lo dominas con 8 de 10).';
            })
            .catch(function () { });
    }

    // --- 1. Escucha y elige ---
    var botonEmpezar = document.getElementById('btn-empezar-escuchar');
    var ronda = document.getElementById('ronda-escuchar');
    var opciones = document.getElementById('opciones-escuchar');
    var mensajeEscuchar = document.getElementById('mensaje-escuchar');
    var numeroRonda = 0, aciertosRonda = 0, actualEscuchar = null;

    function siguienteRonda() {
        if (numeroRonda >= RONDAS) {
            document.getElementById('numero-ronda').textContent = '';
            opciones.textContent = '';
            mensajeEscuchar.textContent = 'Acertaste ' + aciertosRonda + ' de ' + RONDAS + '.' +
                (aciertosRonda === RONDAS ? ' ¡Perfecto! 🎉' : ' Escucha otra vez los pares de arriba y repite.');
            botonEmpezar.textContent = 'Otra vez';
            botonEmpezar.hidden = false;
            document.getElementById('btn-repetir-escuchar').hidden = true;
            return;
        }

        numeroRonda++;
        actualEscuchar = elegirPalabra();
        document.getElementById('numero-ronda').textContent = 'Ronda ' + numeroRonda + ' de ' + RONDAS;
        mensajeEscuchar.textContent = '';
        opciones.textContent = '';
        // Las dos palabras del par, en orden al azar.
        var orden = Math.random() < 0.5 ? actualEscuchar.par : [actualEscuchar.par[1], actualEscuchar.par[0]];
        orden.forEach(function (palabra) {
            var boton = document.createElement('button');
            boton.type = 'button';
            boton.className = 'btn btn-outline-primary btn-lg opcion-sonido';
            boton.textContent = palabra;
            boton.addEventListener('click', function () { elegir(boton, palabra); });
            opciones.appendChild(boton);
        });
        hablar(actualEscuchar.palabra, false);
    }

    function elegir(boton, palabra) {
        var acerto = palabra === actualEscuchar.palabra;
        opciones.querySelectorAll('button').forEach(function (b) {
            b.disabled = true;
            if (b.textContent === actualEscuchar.palabra) {
                b.classList.add('correcta');
            }
        });
        if (!acerto) {
            boton.classList.add('incorrecta');
        }
        aciertosRonda += acerto ? 1 : 0;
        mensajeEscuchar.textContent = acerto ? '¡Bien!' : 'Era "' + actualEscuchar.palabra + '".';
        guardar('Escuchar', acerto);
        setTimeout(siguienteRonda, acerto ? 900 : 1800);
    }

    if (!hayVoz) {
        document.getElementById('escuchar-sin-voz').hidden = false;
        botonEmpezar.hidden = true;
    }

    botonEmpezar.addEventListener('click', function () {
        numeroRonda = 0;
        aciertosRonda = 0;
        botonEmpezar.hidden = true;
        ronda.hidden = false;
        document.getElementById('btn-repetir-escuchar').hidden = false;
        siguienteRonda();
    });

    document.getElementById('btn-repetir-escuchar').addEventListener('click', function () {
        if (actualEscuchar) {
            hablar(actualEscuchar.palabra, true);
        }
    });

    // --- 2. Dila tú ---
    var palabraDecir = document.getElementById('palabra-decir');
    var mensajeDecir = document.getElementById('mensaje-decir');
    var botonDecir = document.getElementById('btn-decir');
    var actualDecir = null;

    function otraPalabra() {
        actualDecir = elegirPalabra();
        palabraDecir.textContent = actualDecir.palabra;
        mensajeDecir.textContent = '';
        mensajeDecir.className = 'mb-0 mt-2';
    }

    otraPalabra();
    document.getElementById('btn-otra-decir').addEventListener('click', otraPalabra);
    document.getElementById('btn-escuchar-decir').addEventListener('click', function () {
        if (hayVoz) {
            hablar(actualDecir.palabra, true);
        }
    });

    if (!Reconocedor) {
        document.getElementById('decir-sin-voz').hidden = false;
        botonDecir.disabled = true;
        return;
    }

    botonDecir.addEventListener('click', function () {
        var reconocimiento = new Reconocedor();
        reconocimiento.lang = 'en-US';
        reconocimiento.interimResults = false;
        reconocimiento.maxAlternatives = 5;
        var huboResultado = false;

        reconocimiento.onstart = function () {
            botonDecir.disabled = true;
            mensajeDecir.className = 'mb-0 mt-2 text-muted';
            mensajeDecir.textContent = 'Escuchando… di "' + actualDecir.palabra + '".';
        };

        reconocimiento.onresult = function (evento) {
            huboResultado = true;
            var alternativas = [];
            for (var i = 0; i < evento.results[0].length; i++) {
                alternativas.push(evento.results[0][i].transcript);
            }
            var resultado = Sonidos.evaluar(actualDecir.palabra, actualDecir.contra, alternativas);
            if (resultado === 'bien') {
                mensajeDecir.className = 'mb-0 mt-2 practica-ok';
                mensajeDecir.textContent = '¡Bien! Se entendió "' + actualDecir.palabra + '".';
                guardar('Decir', true);
            } else if (resultado === 'contra') {
                mensajeDecir.className = 'mb-0 mt-2 practica-falta';
                mensajeDecir.textContent = 'Se entendió "' + actualDecir.contra + '" en vez de "' + actualDecir.palabra +
                    '". Revisa "Cómo se hace" e intenta otra vez.';
                guardar('Decir', false);
            } else {
                // Si no se entendió ninguna de las dos, no cuenta: puede ser el micrófono o el ruido.
                mensajeDecir.className = 'mb-0 mt-2 text-muted';
                mensajeDecir.textContent = 'Entendí "' + alternativas[0] + '". No cuenta; intenta otra vez, más claro.';
            }
        };

        reconocimiento.onerror = function (evento) {
            huboResultado = true;
            mensajeDecir.className = 'mb-0 mt-2 text-muted';
            mensajeDecir.textContent = evento.error === 'not-allowed' || evento.error === 'service-not-allowed'
                ? 'Necesito permiso para usar el micrófono.'
                : 'No te escuché. Intenta otra vez.';
        };

        reconocimiento.onend = function () {
            botonDecir.disabled = false;
            if (!huboResultado) {
                mensajeDecir.textContent = 'No te escuché. Intenta otra vez.';
            }
        };

        window.speechSynthesis && window.speechSynthesis.cancel();
        reconocimiento.start();
    });
})();
