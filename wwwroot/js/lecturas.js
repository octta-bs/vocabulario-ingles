// Leer en contexto: tocar una palabra abre un panel con su frase; al guardar, entra al repaso con
// el significado que escribiste (o el que ya tenía en la app).
(function () {
    var lectura = document.getElementById('lectura');
    if (!lectura) {
        return;
    }

    var oraciones = JSON.parse(document.getElementById('oraciones').textContent);
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    var panel = document.getElementById('panel-palabra');
    var titulo = document.getElementById('panel-titulo');
    var frase = document.getElementById('panel-frase');
    var significado = document.getElementById('panel-significado');
    var ayuda = document.getElementById('panel-ayuda');
    var mensaje = document.getElementById('panel-mensaje');
    var botonGuardar = document.getElementById('panel-guardar');
    var palabraActual = '';
    var oracionActual = '';

    function marcar(palabra) {
        var clave = palabra.toLowerCase();
        lectura.querySelectorAll('.palabra-lectura').forEach(function (b) {
            if (b.textContent.toLowerCase() === clave) {
                b.classList.add('aprendiendo');
            }
        });
    }

    // Muestra la frase con la palabra resaltada (con nodos de texto, nunca HTML del texto pegado).
    function mostrarFrase(texto, palabra) {
        frase.textContent = '';
        var i = texto.indexOf(palabra);
        if (i < 0) {
            frase.textContent = texto;
            return;
        }
        frase.appendChild(document.createTextNode(texto.slice(0, i)));
        var marca = document.createElement('mark');
        marca.textContent = palabra;
        frase.appendChild(marca);
        frase.appendChild(document.createTextNode(texto.slice(i + palabra.length)));
    }

    lectura.addEventListener('click', function (evento) {
        var boton = evento.target.closest('.palabra-lectura');
        if (!boton) {
            return;
        }

        palabraActual = boton.textContent;
        oracionActual = oraciones[+boton.getAttribute('data-oracion')] || '';
        titulo.textContent = palabraActual.toLowerCase();
        document.getElementById('panel-audio').setAttribute('data-texto', palabraActual);
        mostrarFrase(oracionActual, palabraActual);
        significado.value = '';
        significado.disabled = false;
        ayuda.textContent = 'Escríbelo en español. Si no lo sabes, búscalo y anótalo aquí.';
        mensaje.textContent = '';
        mensaje.className = 'small mb-2';
        botonGuardar.disabled = false;
        panel.showModal();
        significado.focus();

        // Si la palabra ya existe en la app, su significado aparece solo.
        fetch(lectura.getAttribute('data-significado') + '?palabra=' + encodeURIComponent(palabraActual))
            .then(function (r) { return r.json(); })
            .then(function (datos) {
                if (datos.significado && titulo.textContent === palabraActual.toLowerCase()) {
                    significado.value = datos.significado;
                    ayuda.textContent = 'Ya existe en la app con este significado.';
                }
            })
            .catch(function () { });
    });

    botonGuardar.addEventListener('click', function () {
        var datos = new URLSearchParams();
        datos.append('palabra', palabraActual);
        datos.append('frase', oracionActual);
        datos.append('significado', significado.value);
        datos.append('__RequestVerificationToken', token);
        botonGuardar.disabled = true;

        fetch(lectura.getAttribute('data-guardar'), { method: 'POST', body: datos })
            .then(function (r) { return r.json(); })
            .then(function (resultado) {
                mensaje.textContent = resultado.mensaje;
                mensaje.className = 'small mb-2 ' + (resultado.guardada ? 'text-success' : 'text-danger');
                if (resultado.guardada) {
                    marcar(palabraActual);
                    setTimeout(function () { panel.close(); }, 900);
                } else {
                    botonGuardar.disabled = false;
                    significado.focus();
                }
            })
            .catch(function () {
                mensaje.textContent = 'No se pudo guardar. Intenta otra vez.';
                mensaje.className = 'small mb-2 text-danger';
                botonGuardar.disabled = false;
            });
    });

    significado.addEventListener('keydown', function (evento) {
        if (evento.key === 'Enter') {
            evento.preventDefault();
            botonGuardar.click();
        }
    });
})();
