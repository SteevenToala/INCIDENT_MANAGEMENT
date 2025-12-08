namespace IncidentManagement.Application.DTOs;

public class AsignacionDto
{
    public int AsignacionID { get; set; }
    public int IncidenteID { get; set; }
    public int? UsuarioAsignadoID { get; set; }
    public string? TipoAsignacion { get; set; }
    public string EstadoAsignacion { get; set; } = "Pendiente";
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaCompletacion { get; set; }
    public string? Notas { get; set; }
    public string? NombreUsuarioAsignado { get; set; }
    public string? NombreTecnico { get; set; }
}

public class CreateAsignacionDto
{
    public int IncidenteID { get; set; }
    public int UsuarioAsignadoID { get; set; }
    public string TipoAsignacion { get; set; } = "Laboratorista";
    public string? Notas { get; set; }
}

public class AceptarAsignacionDto
{
    public int AsignacionID { get; set; }
}

public class CompletarAsignacionDto
{
    public int AsignacionID { get; set; }
    public string? Notas { get; set; }
}
