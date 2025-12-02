using IncidentManagement.Application.DTOs;

namespace Web.Models.Estudiante;

public class EstudianteDashboardModel
{
    public DashboardCounters Counters { get; init; } = new();
    public IReadOnlyList<IncidenteDto> UltimasIncidencias { get; init; } = Array.Empty<IncidenteDto>();
}

public class DashboardCounters
{
    public int Total { get; init; }
    public int Pendientes { get; init; }
    public int EnProceso { get; init; }
    public int Resueltos { get; init; }
}
