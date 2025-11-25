using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface IIncidenteService
{
    Task<IEnumerable<IncidenteDto>> GetAllIncidentesAsync();
    Task<IEnumerable<IncidenteDto>> GetMisIncidentesAsync(int usuarioId);
    Task<IncidenteDto?> GetIncidenteByIdAsync(int id);
    Task<IncidenteDto> CreateIncidenteAsync(CreateIncidenteDto dto, int usuarioId);
    Task UpdateIncidenteAsync(UpdateIncidenteDto dto);
    Task DeleteIncidenteAsync(int id, int usuarioId);
}
