using IncidentManagement.Application.Interfaces;
using IncidentManagement.Application.DTOs;
using WebPush;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace IncidentManagement.Application.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly ISuscripcionPushRepository _suscripcionRepository;
    private readonly WebPushClient _webPushClient;
    private readonly string _vapidPublicKey;
    private readonly string _vapidPrivateKey;
    private readonly string _vapidSubject;

    public PushNotificationService(
        ISuscripcionPushRepository suscripcionRepository,
        IConfiguration configuration)
    {
        _suscripcionRepository = suscripcionRepository;
        _webPushClient = new WebPushClient();
        
        // Las claves VAPID para notificaciones push
        _vapidPublicKey = configuration["PushNotifications:VapidPublicKey"] ?? throw new InvalidOperationException("VAPID public key not configured");
        _vapidPrivateKey = configuration["PushNotifications:VapidPrivateKey"] ?? throw new InvalidOperationException("VAPID private key not configured");
        _vapidSubject = configuration["PushNotifications:VapidSubject"] ?? "mailto:admin@incidentmanagement.com";
    }

    public async Task<bool> SendNotificationAsync(int usuarioId, string titulo, string mensaje, string? urlAccion = null)
    {
        var suscripciones = await _suscripcionRepository.GetActivasByUsuarioAsync(usuarioId);
        
        if (!suscripciones.Any())
            return false;

        var payload = new PushNotificationPayloadDto
        {
            Titulo = titulo,
            Mensaje = mensaje,
            URLAccion = urlAccion,
            Icono = "/icon-192.png",
            Tipo = "info"
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var success = true;

        foreach (var suscripcion in suscripciones)
        {
            try
            {
                var pushSubscription = new PushSubscription(
                    suscripcion.Endpoint,
                    suscripcion.P256DH,
                    suscripcion.Auth
                );

                var vapidDetails = new VapidDetails(_vapidSubject, _vapidPublicKey, _vapidPrivateKey);

                await _webPushClient.SendNotificationAsync(pushSubscription, jsonPayload, vapidDetails);
            }
            catch (WebPushException ex)
            {
                // Si la suscripción ya no es válida (410 Gone), desactivarla
                if (ex.StatusCode == System.Net.HttpStatusCode.Gone)
                {
                    suscripcion.Activa = false;
                    await _suscripcionRepository.UpdateAsync(suscripcion);
                }
                success = false;
            }
            catch
            {
                success = false;
            }
        }

        return success;
    }

    public async Task<bool> SendNotificationToMultipleUsersAsync(
        IEnumerable<int> usuarioIds, 
        string titulo, 
        string mensaje, 
        string? urlAccion = null)
    {
        var tasks = usuarioIds.Select(id => SendNotificationAsync(id, titulo, mensaje, urlAccion));
        var results = await Task.WhenAll(tasks);
        return results.Any(r => r);
    }
}
