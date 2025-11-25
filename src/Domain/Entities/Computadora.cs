namespace IncidentManagement.Domain.Entities;

public class Computadora
{
    public int ComputadoraID { get; set; }
    public int LaboratorioID { get; set; }
    public string CodigoEquipo { get; set; } = string.Empty;
    public string? NumeroSerie { get; set; }
    public string? Modelo { get; set; }
    public string? SistemaOperativo { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // Navigation properties
    public virtual Laboratorio Laboratorio { get; set; } = null!;
    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
