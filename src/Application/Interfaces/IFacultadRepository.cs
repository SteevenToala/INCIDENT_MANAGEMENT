using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IFacultadRepository
{
    Task<IEnumerable<Facultad>> GetAllAsync();
    Task<Facultad?> GetByIdAsync(int id);
}
