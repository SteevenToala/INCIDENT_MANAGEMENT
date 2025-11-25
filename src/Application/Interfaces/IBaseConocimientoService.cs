using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface IBaseConocimientoService
{
    Task<IEnumerable<BaseConocimientoDto>> GetAllAsync();
    Task<IEnumerable<BaseConocimientoDto>> SearchAsync(string searchTerm);
    Task<BaseConocimientoDto?> GetByIdAsync(int id);
    Task<BaseConocimientoDto> CreateAsync(CreateBaseConocimientoDto dto, int usuarioCreadorId);
}
