using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface IIncidenteService
{
    Task<IEnumerable<IncidenteDto>> GetAllIncidentesAsync();
    Task<IEnumerable<IncidenteDto>> GetMisIncidentesAsync(int usuarioId);
    Task<IEnumerable<IncidenteDto>> GetIncidentesByTecnicoAsync(int tecnicoId);
    Task<IncidenteDto?> GetIncidenteByIdAsync(int id);
    Task<IncidenteDto?> GetByIdAsync(int id);
    Task<IncidenteDto> CreateIncidenteAsync(CreateIncidenteDto dto, int usuarioId);
    Task UpdateIncidenteAsync(UpdateIncidenteDto dto);
    Task ActualizarEstadoAsync(ActualizarEstadoDto dto);
    Task DeleteIncidenteAsync(int id, int usuarioId);
}
