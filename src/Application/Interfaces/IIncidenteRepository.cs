using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IIncidenteRepository
{
    Task<Incidente?> GetByIdAsync(int id);
    Task<Incidente?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Incidente>> GetAllAsync();
    Task<IEnumerable<Incidente>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Incidente>> GetByEstadoAsync(string estado);
    Task<IEnumerable<Incidente>> GetByLaboratorioAsync(int laboratorioId);
    Task<IEnumerable<Incidente>> GetByFacultadAsync(int facultadId);
    Task<Incidente> CreateAsync(Incidente incidente);
    Task UpdateAsync(Incidente incidente);
    Task DeleteAsync(int id);
}
