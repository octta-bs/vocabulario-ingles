// Service worker mínimo: solo sirve para que el navegador permita instalar la página como app.
// Todavía no guarda nada para usarse sin internet.
self.addEventListener('install', function () {
    self.skipWaiting();
});

self.addEventListener('activate', function (evento) {
    evento.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', function () {
    // Sin caché por ahora: todo se pide normalmente a la red.
});
