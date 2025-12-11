using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Services;

public class AsignacionService : IAsignacionService
{
    private readonly IAsignacionRepository _asignacionRepository;
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly INotificacionService _notificacionService;

    public AsignacionService(
        IAsignacionRepository asignacionRepository,
        IIncidenteRepository incidenteRepository,
        INotificacionService notificacionService)
    {
        _asignacionRepository = asignacionRepository;
        _incidenteRepository = incidenteRepository;
        _notificacionService = notificacionService;
    }

    public async Task<AsignacionDto> AsignarIncidenteAsync(CreateAsignacionDto dto)
    {
        // Crear la asignación
        var asignacion = new AsignacionIncidente
        {
            IncidenteID = dto.IncidenteID,
            UsuarioAsignadoID = dto.UsuarioAsignadoID,
            TipoAsignacion = dto.TipoAsignacion,
            EstadoAsignacion = "Pendiente",
            FechaAsignacion = DateTime.Now,
            Notas = dto.Notas
        };

        var asignacionCreada = await _asignacionRepository.CreateAsync(asignacion);

        // Actualizar el estado del incidente a "En Proceso"
        var incidente = await _incidenteRepository.GetByIdAsync(dto.IncidenteID);
        if (incidente != null)
        {
            incidente.Estado = "En Proceso";
            incidente.FechaActualizacion = DateTime.Now;
            await _incidenteRepository.UpdateAsync(incidente);
        }

        // Crear notificación para el técnico asignado (esto también enviará push notification)
        await _notificacionService.CrearNotificacionAsync(
            dto.UsuarioAsignadoID,
            dto.IncidenteID,
            "Nueva tarea asignada",
            $"Se te ha asignado el incidente {incidente?.CodigoIncidente}",
            "asignacion",
            $"/laboratorista/asignaciones/actualizar?id={dto.IncidenteID}"
        );

        return MapToDto(asignacionCreada);
    }

    public async Task AceptarAsignacionAsync(int asignacionId, int usuarioId)
    {
        var asignacion = await _asignacionRepository.GetByIdAsync(asignacionId);
        if (asignacion == null)
            throw new Exception("Asignación no encontrada");

        if (asignacion.UsuarioAsignadoID != usuarioId)
            throw new Exception("No tienes permiso para aceptar esta asignación");

        asignacion.EstadoAsignacion = "En Progreso";
        asignacion.FechaAceptacion = DateTime.Now;

        await _asignacionRepository.UpdateAsync(asignacion);
    }

    public async Task CompletarAsignacionAsync(int asignacionId, string? notas)
    {
        var asignacion = await _asignacionRepository.GetByIdAsync(asignacionId);
        if (asignacion == null)
            throw new Exception("Asignación no encontrada");

        asignacion.EstadoAsignacion = "Completado";
        asignacion.FechaCompletacion = DateTime.Now;
        if (!string.IsNullOrEmpty(notas))
            asignacion.Notas = notas;

        await _asignacionRepository.UpdateAsync(asignacion);
    }

    public async Task EliminarAsignacionAsync(int asignacionId)
    {
        var asignacion = await _asignacionRepository.GetByIdAsync(asignacionId);
        if (asignacion == null)
            throw new Exception("Asignación no encontrada");

        // Verificar que esté en estado Pendiente para poder eliminar
        if (asignacion.EstadoAsignacion != "Pendiente")
            throw new Exception("Solo se pueden rechazar asignaciones en estado Pendiente");

        await _asignacionRepository.DeleteAsync(asignacionId);
    }

    public async Task<IEnumerable<AsignacionDto>> GetAsignacionesByIncidenteAsync(int incidenteId)
    {
        var asignaciones = await _asignacionRepository.GetByIncidenteIdAsync(incidenteId);
        return asignaciones.Select(MapToDto);
    }

    public async Task<IEnumerable<AsignacionDto>> GetAsignacionesByUsuarioAsync(int usuarioId)
    {
        var asignaciones = await _asignacionRepository.GetByUsuarioAsignadoIdAsync(usuarioId);
        return asignaciones.Select(MapToDto);
    }

    private AsignacionDto MapToDto(AsignacionIncidente asignacion)
    {
        return new AsignacionDto
        {
            AsignacionID = asignacion.AsignacionID,
            IncidenteID = asignacion.IncidenteID,
            UsuarioAsignadoID = asignacion.UsuarioAsignadoID,
            TipoAsignacion = asignacion.TipoAsignacion,
            EstadoAsignacion = asignacion.EstadoAsignacion,
            FechaAsignacion = asignacion.FechaAsignacion,
            FechaAceptacion = asignacion.FechaAceptacion,
            FechaCompletacion = asignacion.FechaCompletacion,
            Notas = asignacion.Notas,
            NombreUsuarioAsignado = asignacion.UsuarioAsignado?.NombreCompleto,
            NombreTecnico = asignacion.UsuarioAsignado?.NombreCompleto
        };
    }
}
