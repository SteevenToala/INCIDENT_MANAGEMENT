using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface INotificacionService
{
    Task<IEnumerable<NotificacionDto>> GetNotificacionesByUsuarioAsync(int usuarioId);
    Task<IEnumerable<NotificacionDto>> GetNotificacionesNoLeidasAsync(int usuarioId);
    Task<NotificacionDto?> GetNotificacionByIdAsync(int notificacionId);
    Task MarcarComoLeidaAsync(int notificacionId);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
    Task<int> GetContadorNoLeidasAsync(int usuarioId);
    Task CrearNotificacionAsync(int usuarioId, int? incidenteId, string titulo, string mensaje, string tipo, string? urlAccion = null);
}
