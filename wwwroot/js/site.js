// Lee un texto en inglés con la voz del navegador (sin costo, sin archivos de audio).
function hablar(texto, lento) {
    var voz = new SpeechSynthesisUtterance(texto);
    voz.lang = 'en-US';
    voz.rate = lento ? 0.6 : 0.9;
    window.speechSynthesis.cancel();
    window.speechSynthesis.speak(voz);
}

// Botón de audio. Con data-lento lee más despacio (útil en el dictado).
document.addEventListener('click', function (evento) {
    var boton = evento.target.closest('.btn-audio');
    if (!boton) {
        return;
    }

    var texto = boton.getAttribute('data-texto');
    if (!texto || !('speechSynthesis' in window)) {
        alert('Tu navegador no tiene voz disponible.');
        return;
    }

    hablar(texto, boton.hasAttribute('data-lento'));

    // Después de escuchar, regresa el cursor a la caja de texto para seguir escribiendo.
    var caja = document.querySelector('input[name=respuesta]');
    if (caja) {
        caja.focus();
    }
});

// En el dictado intenta leer el texto al abrir la página. Algunos navegadores no dejan
// reproducir sonido sin que toques algo antes; en ese caso basta con el botón "Escuchar".
var botonAutomatico = document.querySelector('.btn-audio[data-autoplay]');
if (botonAutomatico && 'speechSynthesis' in window) {
    hablar(botonAutomatico.getAttribute('data-texto'), false);
}

// Permite instalar la página en el celular como si fuera una app.
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw.js').catch(function () { });
}
