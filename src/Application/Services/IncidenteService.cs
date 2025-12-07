using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Services;

public class IncidenteService : IIncidenteService
{
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly IComputadoraRepository _computadoraRepository;
    private readonly INotificacionRepository _notificacionRepository;

    public IncidenteService(
        IIncidenteRepository incidenteRepository,
        IComputadoraRepository computadoraRepository,
        INotificacionRepository notificacionRepository)
    {
        _incidenteRepository = incidenteRepository;
        _computadoraRepository = computadoraRepository;
        _notificacionRepository = notificacionRepository;
    }

    public async Task<IEnumerable<IncidenteDto>> GetAllIncidentesAsync()
    {
        var incidentes = await _incidenteRepository.GetAllAsync();
        return incidentes.Select(MapToDto);
    }

    public async Task<IEnumerable<IncidenteDto>> GetMisIncidentesAsync(int usuarioId)
    {
        var incidentes = await _incidenteRepository.GetByUsuarioAsync(usuarioId);
        return incidentes.Select(MapToDto);
    }

    public async Task<IEnumerable<IncidenteDto>> GetIncidentesByTecnicoAsync(int tecnicoId)
    {
        // Obtener todas las incidencias para que el laboratorista pueda verlas y asignarlas
        var incidentes = await _incidenteRepository.GetAllAsync();
        return incidentes.Select(MapToDto);
    }

    public async Task<IncidenteDto?> GetIncidenteByIdAsync(int id)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(id);
        return incidente != null ? MapToDto(incidente) : null;
    }

    public async Task<IncidenteDto?> GetByIdAsync(int id)
    {
        return await GetIncidenteByIdAsync(id);
    }

    public async Task<IncidenteDto> CreateIncidenteAsync(CreateIncidenteDto dto, int usuarioId)
    {
        var computadora = await _computadoraRepository.GetByIdAsync(dto.ComputadoraID);
        if (computadora == null)
            throw new Exception("Computadora no encontrada");

        var codigo = GenerarCodigoIncidente();

        var incidente = new Incidente
        {
            CodigoIncidente = codigo,
            ComputadoraID = dto.ComputadoraID,
            UsuarioReportadorID = usuarioId,
            LaboratorioID = computadora.LaboratorioID,
            FacultadID = computadora.Laboratorio?.FacultadID,
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            Estado = "Abierto",
            FechaCreacion = DateTime.Now,
            FechaActualizacion = DateTime.Now
        };

        var creado = await _incidenteRepository.CreateAsync(incidente);
        return MapToDto(creado);
    }

    public async Task UpdateIncidenteAsync(UpdateIncidenteDto dto)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(dto.IncidenteID);
        if (incidente == null)
            throw new Exception("Incidente no encontrado");

        if (!string.IsNullOrEmpty(dto.Estado))
            incidente.Estado = dto.Estado;

        if (!string.IsNullOrEmpty(dto.Prioridad))
            incidente.Prioridad = dto.Prioridad;

        if (!string.IsNullOrEmpty(dto.Descripcion))
            incidente.Descripcion = dto.Descripcion;

        incidente.FechaActualizacion = DateTime.Now;

        if (dto.Estado == "Cerrado" || dto.Estado == "Resuelto")
            incidente.FechaResolucion = DateTime.Now;

        await _incidenteRepository.UpdateAsync(incidente);
    }

    public async Task DeleteIncidenteAsync(int id, int usuarioId)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(id);
        if (incidente == null)
            throw new Exception("Incidente no encontrado");

        if (incidente.UsuarioReportadorID != usuarioId)
            throw new Exception("No tiene permisos para eliminar este incidente");

        if (incidente.Estado != "Abierto")
            throw new Exception("No se puede eliminar un incidente en progreso");

        incidente.Eliminado = true;
        await _incidenteRepository.UpdateAsync(incidente);
    }

    public async Task ActualizarEstadoAsync(ActualizarEstadoDto dto)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(dto.IncidenteID);
        if (incidente == null)
            throw new Exception("Incidente no encontrado");

        incidente.Estado = dto.Estado;
        incidente.FechaActualizacion = DateTime.Now;

        if (dto.Estado == "Cerrado" || dto.Estado == "Resuelto")
            incidente.FechaResolucion = DateTime.Now;

        await _incidenteRepository.UpdateAsync(incidente);
    }

    private IncidenteDto MapToDto(Incidente incidente)
    {
        return new IncidenteDto
        {
            IncidenteID = incidente.IncidenteID,
            CodigoIncidente = incidente.CodigoIncidente,
            Titulo = incidente.Titulo,
            Descripcion = incidente.Descripcion,
            Prioridad = incidente.Prioridad,
            Estado = incidente.Estado,
            FechaReporte = incidente.FechaCreacion,
            FechaCreacion = incidente.FechaCreacion,
            FechaResolucion = incidente.FechaResolucion,
            TecnicoID = incidente.Asignaciones.FirstOrDefault(a => a.FechaCompletacion == null)?.UsuarioAsignadoID,
            ComputadoraNombre = incidente.Computadora?.CodigoEquipo ?? "",
            LaboratorioNombre = incidente.Laboratorio?.Nombre ?? "",
            UsuarioReportador = incidente.UsuarioReportador?.NombreCompleto ?? "",
            EstudianteNombre = incidente.UsuarioReportador?.NombreCompleto ?? "",
            RolReportador = incidente.UsuarioReportador?.Rol?.Nombre ?? ""
        };
    }

    private string GenerarCodigoIncidente()
    {
        return $"INC-{DateTime.Now:yyyyMMddHHmmss}";
    }
}
