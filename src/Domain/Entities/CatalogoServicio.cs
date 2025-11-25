namespace IncidentManagement.Domain.Entities;

public class CatalogoServicio
{
    public int ServicioID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? TipoSolucion { get; set; }
    public int? TiempoResolucionPromedio { get; set; }
    public string? UsuariosPermitidos { get; set; }
    public int? ResponsableUsuarioID { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // Navigation properties
    public virtual Usuario? ResponsableUsuario { get; set; }
    public virtual ICollection<ContratoSLA> ContratosSLA { get; set; } = new List<ContratoSLA>();
    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
