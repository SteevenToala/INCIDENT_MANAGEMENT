namespace IncidentManagement.Domain.Entities;

public class Laboratorio
{
    public int LaboratorioID { get; set; }
    public int FacultadID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public int? Capacidad { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual Facultad Facultad { get; set; } = null!;
    public virtual ICollection<Computadora> Computadoras { get; set; } = new List<Computadora>();
    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
