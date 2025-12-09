using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface ICatalogoServicioRepository
{
    Task<IEnumerable<CatalogoServicio>> GetAllAsync();
    Task<CatalogoServicio?> GetByIdAsync(int id);
}
