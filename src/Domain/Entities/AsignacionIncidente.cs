namespace IncidentManagement.Domain.Entities;

public class AsignacionIncidente
{
    public int AsignacionID { get; set; }
    public int IncidenteID { get; set; }
    public int? UsuarioAsignadoID { get; set; }
    public string? TipoAsignacion { get; set; }
    public string EstadoAsignacion { get; set; } = "Pendiente";
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaCompletacion { get; set; }
    public string? Notas { get; set; }

    // Navigation properties
    public virtual Incidente Incidente { get; set; } = null!;
    public virtual Usuario? UsuarioAsignado { get; set; }
}
