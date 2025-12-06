namespace IncidentManagement.Application.DTOs;

public class IncidenteDto
{
    public int IncidenteID { get; set; }
    public string CodigoIncidente { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaReporte { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public int? TecnicoID { get; set; }
    public string ComputadoraNombre { get; set; } = string.Empty;
    public string LaboratorioNombre { get; set; } = string.Empty;
    public string UsuarioReportador { get; set; } = string.Empty;
    public string EstudianteNombre { get; set; } = string.Empty;
}

public class CreateIncidenteDto
{
    public int ComputadoraID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = "Media";
}

public class UpdateIncidenteDto
{
    public int IncidenteID { get; set; }
    public string? Estado { get; set; }
    public string? Prioridad { get; set; }
    public string? Descripcion { get; set; }
}

public class ActualizarEstadoDto
{
    public int IncidenteID { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Comentarios { get; set; }
}
