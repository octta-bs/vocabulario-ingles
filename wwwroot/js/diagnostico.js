// Diagnóstico de nivel: muestra las preguntas una por una (sin JavaScript se ven todas juntas).
// No dice si acertaste en cada una: el resultado sale al final.
(function () {
    var formulario = document.getElementById('diagnostico');
    if (!formulario) {
        return;
    }

    var preguntas = Array.prototype.slice.call(formulario.querySelectorAll('.pregunta-diagnostico'));
    var botonSiguiente = document.getElementById('btn-siguiente-diagnostico');
    var botonTerminar = document.getElementById('btn-terminar-diagnostico');
    var numero = document.getElementById('numero-pregunta');
    var avance = document.getElementById('avance-diagnostico');
    var actual = 0;

    function mostrar(indice) {
        preguntas.forEach(function (p, i) { p.hidden = i !== indice; });
        var pregunta = preguntas[indice];
        var ultima = indice === preguntas.length - 1;
        botonSiguiente.hidden = ultima;
        botonTerminar.hidden = !ultima;
        numero.textContent = (indice + 1) + ' de ' + preguntas.length;
        avance.style.width = Math.round(indice * 100 / preguntas.length) + '%';

        // En el dictado, el audio suena solo al llegar a la pregunta.
        var audio = pregunta.querySelector('.btn-audio:not([data-lento])');
        if (audio && typeof hablar === 'function' && 'speechSynthesis' in window) {
            hablar(audio.getAttribute('data-texto'), false);
        }
        pregunta.querySelector('input[name=respuesta]').focus();
    }

    function siguiente() {
        if (actual < preguntas.length - 1) {
            actual++;
            mostrar(actual);
        }
    }

    botonSiguiente.addEventListener('click', siguiente);

    // Enter pasa a la siguiente pregunta; en la última, envía el examen.
    formulario.addEventListener('keydown', function (evento) {
        if (evento.key === 'Enter' && evento.target.name === 'respuesta' && actual < preguntas.length - 1) {
            evento.preventDefault();
            siguiente();
        }
    });

    formulario.addEventListener('submit', function () {
        botonTerminar.disabled = true;
        botonTerminar.textContent = 'Calificando…';
    });

    if (preguntas.length > 0) {
        mostrar(0);
    }
})();
