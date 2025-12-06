using System;
using System.Linq;
using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using Web.Models.Estudiante;

namespace Web.Services;

public interface IEstudiantePortalService
{
    Task<EstudianteDashboardModel> GetDashboardAsync(int usuarioId);
    Task<IReadOnlyList<IncidenteDto>> GetMisIncidentesAsync(int usuarioId);
    Task<IncidenteDto> CrearIncidenteAsync(NuevaIncidenciaModel model, int usuarioId);
    Task<IReadOnlyList<BaseConocimientoDto>> BuscarBaseConocimientoAsync(string? filtro);
    Task<IReadOnlyList<ComputadoraOption>> GetComputadorasAsync();
}

public class EstudiantePortalService : IEstudiantePortalService
{
    private readonly IIncidenteService _incidenteService;
    private readonly IBaseConocimientoService _baseConocimientoService;
    private readonly IComputadoraRepository _computadoraRepository;

    public EstudiantePortalService(
        IIncidenteService incidenteService,
        IBaseConocimientoService baseConocimientoService,
        IComputadoraRepository computadoraRepository)
    {
        _incidenteService = incidenteService;
        _baseConocimientoService = baseConocimientoService;
        _computadoraRepository = computadoraRepository;
    }

    public async Task<EstudianteDashboardModel> GetDashboardAsync(int usuarioId)
    {
        var incidentes = (await _incidenteService.GetMisIncidentesAsync(usuarioId)).ToList();

        var counters = new DashboardCounters
        {
            Total = incidentes.Count,
            Pendientes = incidentes.Count(i => i.Estado.Equals("Abierto", StringComparison.OrdinalIgnoreCase)),
            EnProceso = incidentes.Count(i => i.Estado.Equals("En Proceso", StringComparison.OrdinalIgnoreCase)),
            Resueltos = incidentes.Count(i => i.Estado.Equals("Resuelto", StringComparison.OrdinalIgnoreCase) || i.Estado.Equals("Cerrado", StringComparison.OrdinalIgnoreCase))
        };

        var ultimas = incidentes
            .OrderByDescending(i => i.FechaCreacion)
            .Take(5)
            .ToList();

        return new EstudianteDashboardModel
        {
            Counters = counters,
            UltimasIncidencias = ultimas
        };
    }

    public async Task<IReadOnlyList<IncidenteDto>> GetMisIncidentesAsync(int usuarioId)
    {
        var incidentes = await _incidenteService.GetMisIncidentesAsync(usuarioId);
        return incidentes
            .OrderByDescending(i => i.FechaCreacion)
            .ToList();
    }

    public async Task<IncidenteDto> CrearIncidenteAsync(NuevaIncidenciaModel model, int usuarioId)
    {
        var dto = new CreateIncidenteDto
        {
            ComputadoraID = model.ComputadoraID!.Value,
            Titulo = model.Titulo,
            Descripcion = model.Descripcion,
            Prioridad = model.Prioridad
        };

        return await _incidenteService.CreateIncidenteAsync(dto, usuarioId);
    }

    public async Task<IReadOnlyList<BaseConocimientoDto>> BuscarBaseConocimientoAsync(string? filtro)
    {
        IEnumerable<BaseConocimientoDto> resultados = string.IsNullOrWhiteSpace(filtro)
            ? await _baseConocimientoService.GetAllAsync()
            : await _baseConocimientoService.SearchAsync(filtro);

        return resultados
            .OrderByDescending(r => r.FechaCreacion)
            .ToList();
    }

    public async Task<IReadOnlyList<ComputadoraOption>> GetComputadorasAsync()
    {
        var computadoras = await _computadoraRepository.GetAllAsync();

        return computadoras
            .Select(c => new ComputadoraOption
            {
                Id = c.ComputadoraID,
                Nombre = c.CodigoEquipo,
                Descripcion = c.Laboratorio != null ? $"{c.Laboratorio.Nombre} - {c.Laboratorio.Ubicacion}" : "Sin laboratorio",
                LaboratorioId = c.Laboratorio?.LaboratorioID ?? 0,
                LaboratorioNombre = c.Laboratorio?.Nombre ?? "Sin laboratorio"
            })
            .OrderBy(c => c.Nombre)
            .ToList();
    }
}
