using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Services;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IPushNotificationService _pushNotificationService;

    public NotificacionService(
        INotificacionRepository notificacionRepository,
        IPushNotificationService pushNotificationService)
    {
        _notificacionRepository = notificacionRepository;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<IEnumerable<NotificacionDto>> GetNotificacionesByUsuarioAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.GetByUsuarioAsync(usuarioId);
        return notificaciones.Select(n => MapToDto(n));
    }

    public async Task<IEnumerable<NotificacionDto>> GetNotificacionesNoLeidasAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.GetNoLeidasAsync(usuarioId);
        return notificaciones.Select(n => MapToDto(n));
    }

    public async Task<NotificacionDto?> GetNotificacionByIdAsync(int notificacionId)
    {
        var notificacion = await _notificacionRepository.GetByIdAsync(notificacionId);
        return notificacion != null ? MapToDto(notificacion) : null;
    }

    public async Task MarcarComoLeidaAsync(int notificacionId)
    {
        await _notificacionRepository.MarcarComoLeidaAsync(notificacionId);
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.GetNoLeidasAsync(usuarioId);
        foreach (var notificacion in notificaciones)
        {
            await _notificacionRepository.MarcarComoLeidaAsync(notificacion.NotificacionID);
        }
    }

    public async Task<int> GetContadorNoLeidasAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.GetNoLeidasAsync(usuarioId);
        return notificaciones.Count();
    }

    public async Task CrearNotificacionAsync(
        int usuarioId, 
        int? incidenteId, 
        string titulo, 
        string mensaje, 
        string tipo, 
        string? urlAccion = null)
    {
        var notificacion = new Notificacion
        {
            UsuarioID = usuarioId,
            IncidenteID = incidenteId,
            Titulo = titulo,
            Mensaje = mensaje,
            Tipo = tipo,
            URLAccion = urlAccion,
            Leida = false,
            FechaCreacion = DateTime.Now
        };

        await _notificacionRepository.CreateAsync(notificacion);

        // Enviar notificación push
        await _pushNotificationService.SendNotificationAsync(usuarioId, titulo, mensaje, urlAccion);
    }

    private static NotificacionDto MapToDto(Notificacion notificacion)
    {
        return new NotificacionDto
        {
            NotificacionID = notificacion.NotificacionID,
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            Tipo = notificacion.Tipo,
            Leida = notificacion.Leida,
            FechaCreacion = notificacion.FechaCreacion,
            URLAccion = notificacion.URLAccion
        };
    }
}
