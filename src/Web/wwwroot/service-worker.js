// Service Worker para notificaciones push
self.addEventListener('install', event => {
    console.log('[Service Worker] Instalado');
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    console.log('[Service Worker] Activado');
    event.waitUntil(self.clients.claim());
});

self.addEventListener('push', event => {
    console.log('[Service Worker] Push recibido', event);
    
    let data = {
        title: 'Nueva notificación',
        body: 'Tienes una nueva actualización',
        icon: '/favicon.ico',
        badge: '/favicon.ico'
    };

    if (event.data) {
        try {
            data = event.data.json();
        } catch (e) {
            data.body = event.data.text();
        }
    }

    const options = {
        body: data.body || data.mensaje || data.Mensaje,
        icon: data.icon || '/favicon.ico',
        badge: data.badge || '/favicon.ico',
        vibrate: [200, 100, 200],
        tag: data.tag || 'notification',
        data: data,
        requireInteraction: false
    };

    event.waitUntil(
        self.registration.showNotification(data.title || data.titulo || data.Titulo, options)
    );
});

self.addEventListener('notificationclick', event => {
    console.log('[Service Worker] Notificación clickeada', event);
    event.notification.close();

    event.waitUntil(
        clients.openWindow('/')
    );
});
