using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface ILaboratorioRepository
{
    Task<IEnumerable<Laboratorio>> GetAllAsync();
    Task<Laboratorio?> GetByIdAsync(int id);
    Task<IEnumerable<Laboratorio>> GetByFacultadAsync(int facultadId);
}
