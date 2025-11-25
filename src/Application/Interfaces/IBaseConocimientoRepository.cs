using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IBaseConocimientoRepository
{
    Task<BaseConocimiento?> GetByIdAsync(int id);
    Task<IEnumerable<BaseConocimiento>> GetAllAsync();
    Task<IEnumerable<BaseConocimiento>> SearchAsync(string searchTerm);
    Task<IEnumerable<BaseConocimiento>> GetByCategoriaAsync(string categoria);
    Task<BaseConocimiento> CreateAsync(BaseConocimiento conocimiento);
    Task UpdateAsync(BaseConocimiento conocimiento);
    Task DeleteAsync(int id);
}
