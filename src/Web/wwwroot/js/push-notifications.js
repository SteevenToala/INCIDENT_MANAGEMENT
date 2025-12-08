// Push Notifications Manager
window.PushNotifications = {
    async initialize(vapidPublicKey) {
        try {
            console.log('[Push] === INICIANDO PROCESO DE SUSCRIPCIÓN ===');
            console.log('[Push] VAPID Key recibida:', vapidPublicKey);

            // Verificar si el navegador soporta notificaciones
            if (!('serviceWorker' in navigator)) {
                console.error('[Push] Service Workers NO soportados en este navegador');
                return { success: false, error: 'Service Workers no soportados' };
            }
            console.log('[Push] ✓ Service Workers soportados');

            if (!('PushManager' in window)) {
                console.error('[Push] Push API NO soportada en este navegador');
                return { success: false, error: 'Push API no soportada' };
            }
            console.log('[Push] ✓ Push API soportada');

            if (!('Notification' in window)) {
                console.error('[Push] Notifications API NO soportada en este navegador');
                return { success: false, error: 'Notifications API no soportada' };
            }
            console.log('[Push] ✓ Notifications API soportada');

            console.log('[Push] Permiso actual de notificaciones:', Notification.permission);

            // Solicitar permiso para notificaciones
            console.log('[Push] Solicitando permiso de notificaciones...');
            const permission = await Notification.requestPermission();
            console.log('[Push] Resultado del permiso:', permission);

            if (permission !== 'granted') {
                console.error('[Push] Permiso DENEGADO o IGNORADO por el usuario');
                return { success: false, error: 'Permiso denegado por el usuario' };
            }
            console.log('[Push] ✓ Permiso CONCEDIDO');

            // Registrar Service Worker
            console.log('[Push] Registrando Service Worker...');
            const registration = await navigator.serviceWorker.register('/service-worker.js');
            console.log('[Push] ✓ Service Worker registrado:', registration);

            // Esperar a que esté activo
            console.log('[Push] Esperando a que Service Worker esté listo...');
            await navigator.serviceWorker.ready;
            console.log('[Push] ✓ Service Worker LISTO');

            // Suscribirse a push notifications
            console.log('[Push] Suscribiéndose a Push Notifications...');
            const subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: this.urlBase64ToUint8Array(vapidPublicKey)
            });

            console.log('[Push] ✓ SUSCRIPCIÓN EXITOSA');
            console.log('[Push] Endpoint:', subscription.endpoint);

            const result = {
                success: true,
                subscription: {
                    endpoint: subscription.endpoint,
                    p256dh: this.arrayBufferToBase64(subscription.getKey('p256dh')),
                    auth: this.arrayBufferToBase64(subscription.getKey('auth'))
                }
            };

            console.log('[Push] === PROCESO COMPLETADO EXITOSAMENTE ===');
            return result;
        } catch (error) {
            console.error('[Push] ❌ ERROR en el proceso:', error);
            console.error('[Push] Error completo:', error.stack);
            return { success: false, error: error.message };
        }
    },

    urlBase64ToUint8Array(base64String) {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding)
            .replace(/\-/g, '+')
            .replace(/_/g, '/');

        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    },

    arrayBufferToBase64(buffer) {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    },

    async getSubscription() {
        try {
            const registration = await navigator.serviceWorker.ready;
            const subscription = await registration.pushManager.getSubscription();
            
            if (!subscription) {
                return null;
            }

            return {
                endpoint: subscription.endpoint,
                p256dh: this.arrayBufferToBase64(subscription.getKey('p256dh')),
                auth: this.arrayBufferToBase64(subscription.getKey('auth'))
            };
        } catch (error) {
            console.error('[Push] Error obteniendo suscripción:', error);
            return null;
        }
    },

    async unsubscribe() {
        try {
            const registration = await navigator.serviceWorker.ready;
            const subscription = await registration.pushManager.getSubscription();
            
            if (subscription) {
                await subscription.unsubscribe();
                console.log('[Push] Desuscrito exitosamente');
                return { success: true };
            }
            
            return { success: false, error: 'No hay suscripción activa' };
        } catch (error) {
            console.error('[Push] Error al desuscribir:', error);
            return { success: false, error: error.message };
        }
    }
};
