using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface INotificacionRepository
{
    Task<Notificacion?> GetByIdAsync(int id);
    Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Notificacion>> GetNoLeidasAsync(int usuarioId);
    Task<Notificacion> CreateAsync(Notificacion notificacion);
    Task UpdateAsync(Notificacion notificacion);
    Task MarcarComoLeidaAsync(int notificacionId);
}
