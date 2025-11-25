namespace IncidentManagement.Domain.Entities;

public class ContratoSLA
{
    public int SLA_ID { get; set; }
    public int ServicioID { get; set; }
    public string? NombreContrato { get; set; }
    public string? Prioridad { get; set; }
    public int? TiempoRespuesta { get; set; }
    public int? TiempoResolucion { get; set; }
    public int? ProveedorUsuarioID { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int Disponibilidad { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual CatalogoServicio Servicio { get; set; } = null!;
    public virtual Usuario? ProveedorUsuario { get; set; }
}
