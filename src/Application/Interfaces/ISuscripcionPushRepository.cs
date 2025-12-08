using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface ISuscripcionPushRepository
{
    Task<SuscripcionPush?> GetByIdAsync(int id);
    Task<IEnumerable<SuscripcionPush>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<SuscripcionPush>> GetActivasByUsuarioAsync(int usuarioId);
    Task<SuscripcionPush> CreateAsync(SuscripcionPush suscripcion);
    Task UpdateAsync(SuscripcionPush suscripcion);
    Task DeleteAsync(int id);
    Task<SuscripcionPush?> GetByEndpointAsync(string endpoint);
}
