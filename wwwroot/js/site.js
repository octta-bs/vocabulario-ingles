// Botón de audio: lee el texto en inglés con la voz del navegador (sin costo, sin archivos de audio).
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

    var voz = new SpeechSynthesisUtterance(texto);
    voz.lang = 'en-US';
    voz.rate = 0.9;
    window.speechSynthesis.cancel();
    window.speechSynthesis.speak(voz);
});

// Permite instalar la página en el celular como si fuera una app.
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/sw.js').catch(function () { });
}
