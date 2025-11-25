namespace IncidentManagement.Domain.Entities;

public class Notificacion
{
    public int NotificacionID { get; set; }
    public int UsuarioID { get; set; }
    public int? IncidenteID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Mensaje { get; set; }
    public string? Tipo { get; set; }
    public bool Leida { get; set; }
    public string? URLAccion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLectura { get; set; }

    // Navigation properties
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Incidente? Incidente { get; set; }
}
