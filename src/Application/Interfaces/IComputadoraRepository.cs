using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IComputadoraRepository
{
    Task<IEnumerable<Computadora>> GetAllAsync();
    Task<Computadora?> GetByIdAsync(int id);
    Task<IEnumerable<Computadora>> GetByLaboratorioAsync(int laboratorioId);
    Task<Computadora?> GetByCodigoAsync(string codigo);
}
