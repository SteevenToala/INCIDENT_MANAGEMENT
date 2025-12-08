using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface IAsignacionService
{
    Task<AsignacionDto> AsignarIncidenteAsync(CreateAsignacionDto dto);
    Task AceptarAsignacionAsync(int asignacionId, int usuarioId);
    Task CompletarAsignacionAsync(int asignacionId, string? notas);
    Task EliminarAsignacionAsync(int asignacionId);
    Task<IEnumerable<AsignacionDto>> GetAsignacionesByIncidenteAsync(int incidenteId);
    Task<IEnumerable<AsignacionDto>> GetAsignacionesByUsuarioAsync(int usuarioId);
}
