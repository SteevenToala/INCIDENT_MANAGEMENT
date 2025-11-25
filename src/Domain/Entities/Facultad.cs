namespace IncidentManagement.Domain.Entities;

public class Facultad
{
    public int FacultadID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual ICollection<Laboratorio> Laboratorios { get; set; } = new List<Laboratorio>();
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
