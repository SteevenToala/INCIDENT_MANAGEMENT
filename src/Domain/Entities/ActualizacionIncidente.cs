namespace IncidentManagement.Domain.Entities;

public class ActualizacionIncidente
{
    public int ActualizacionID { get; set; }
    public int IncidenteID { get; set; }
    public int UsuarioID { get; set; }
    public string? TipoActualizacion { get; set; }
    public string? Descripcion { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual Incidente Incidente { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
