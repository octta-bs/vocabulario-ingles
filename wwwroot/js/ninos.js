// Modo infantil, juego "Escucha y toca": la app dice una palabra en inglés y el niño toca el dibujo.
// Cada respuesta se guarda en su progreso (separado del adulto); si eso falla, el juego sigue igual.
(function () {
    var juego = document.getElementById('juego');
    if (!juego) {
        return;
    }

    var rondas = JSON.parse(document.getElementById('rondas').textContent);
    var nombre = JSON.parse(document.getElementById('nombre-nino').textContent) || '';
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    var urlResponder = juego.getAttribute('data-responder');
    var opciones = document.getElementById('opciones');
    var mensaje = document.getElementById('mensaje');
    var botonSiguiente = document.getElementById('btn-siguiente');
    var puntos = document.querySelectorAll('.ninos-rondas li');
    var indice = 0;
    var aciertos = 0;
    var respondida = false;

    function decir(texto, lento) {
        if (typeof hablar === 'function' && 'speechSynthesis' in window) {
            hablar(texto, lento);
        }
    }

    function registrar(palabraId, acerto) {
        var datos = new URLSearchParams();
        datos.append('palabraId', palabraId);
        datos.append('acerto', acerto ? 'true' : 'false');
        datos.append('__RequestVerificationToken', token);
        fetch(urlResponder, { method: 'POST', body: datos }).catch(function () { });
    }

    function mostrar() {
        var ronda = rondas[indice];
        respondida = false;
        mensaje.textContent = '';
        mensaje.className = 'ninos-mensaje';
        botonSiguiente.hidden = true;
        opciones.textContent = '';

        ronda.opciones.forEach(function (opcion, i) {
            var boton = document.createElement('button');
            boton.type = 'button';
            boton.className = 'ninos-opcion';
            boton.setAttribute('aria-label', 'Opción ' + (i + 1));
            boton.setAttribute('data-id', opcion.id);
            boton.innerHTML = opcion.dibujo; // SVG fijo generado por el servidor (DibujosInfantiles)
            boton.addEventListener('click', function () { elegir(boton, opcion.id); });
            opciones.appendChild(boton);
        });

        puntos.forEach(function (p, i) {
            if (i === indice) {
                p.className = 'actual';
            }
        });
        decir(ronda.ingles, false);
    }

    function elegir(boton, id) {
        if (respondida) {
            return;
        }
        respondida = true;

        var ronda = rondas[indice];
        var acerto = id === ronda.id;
        registrar(ronda.id, acerto);
        opciones.querySelectorAll('button').forEach(function (b) { b.disabled = true; });

        if (acerto) {
            aciertos++;
            boton.classList.add('correcta');
            puntos[indice].className = 'bien';
            mensaje.textContent = '¡Muy bien!';
            mensaje.className = 'ninos-mensaje bien';
            decir('Great job!', false);
            setTimeout(avanzar, 1400);
        } else {
            boton.classList.add('incorrecta');
            var correcta = opciones.querySelector('[data-id="' + ronda.id + '"]');
            if (correcta) {
                correcta.classList.add('correcta');
            }
            puntos[indice].className = 'mal';
            mensaje.textContent = 'Era este. ¡Escucha otra vez!';
            mensaje.className = 'ninos-mensaje mal';
            decir(ronda.ingles, true);
            botonSiguiente.hidden = false;
            botonSiguiente.focus();
        }
    }

    function avanzar() {
        indice++;
        if (indice >= rondas.length) {
            terminar();
        } else {
            mostrar();
        }
    }

    function terminar() {
        juego.hidden = true;
        var estrellas = aciertos === rondas.length ? 3 : aciertos >= Math.ceil(rondas.length * 0.6) ? 2 : 1;
        var contenedor = document.getElementById('estrellas');
        contenedor.setAttribute('aria-label', estrellas + ' de 3 estrellas');
        for (var i = 0; i < 3; i++) {
            var estrella = document.createElement('span');
            estrella.textContent = '★';
            estrella.className = i < estrellas ? 'llena' : 'vacia';
            contenedor.appendChild(estrella);
        }
        document.getElementById('final-titulo').textContent =
            (estrellas === 3 ? '¡Perfecto' : estrellas === 2 ? '¡Muy bien' : '¡Buen intento') + (nombre ? ', ' + nombre : '') + '!';
        document.getElementById('final-texto').textContent = 'Acertaste ' + aciertos + ' de ' + rondas.length + '.';
        document.getElementById('final').hidden = false;
        decir(estrellas === 3 ? 'Perfect!' : 'Well done!', false);
    }

    document.getElementById('btn-escuchar').addEventListener('click', function () {
        if (indice < rondas.length) {
            decir(rondas[indice].ingles, true);
        }
    });
    botonSiguiente.addEventListener('click', avanzar);

    // Los navegadores no dejan sonar audio hasta que se toca algo: el juego empieza con un botón.
    var empezar = document.createElement('button');
    empezar.type = 'button';
    empezar.className = 'btn btn-primary btn-lg btn-bloque ninos-empezar';
    empezar.textContent = '¡Empezar!';
    empezar.addEventListener('click', mostrar);
    opciones.appendChild(empezar);
})();
