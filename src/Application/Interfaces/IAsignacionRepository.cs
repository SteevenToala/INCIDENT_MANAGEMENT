using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IAsignacionRepository
{
    Task<IEnumerable<AsignacionIncidente>> GetAllAsync();
    Task<AsignacionIncidente?> GetByIdAsync(int id);
    Task<IEnumerable<AsignacionIncidente>> GetByIncidenteIdAsync(int incidenteId);
    Task<IEnumerable<AsignacionIncidente>> GetByUsuarioAsignadoIdAsync(int usuarioId);
    Task<AsignacionIncidente> CreateAsync(AsignacionIncidente asignacion);
    Task UpdateAsync(AsignacionIncidente asignacion);
    Task DeleteAsync(int id);
}
