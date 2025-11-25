namespace IncidentManagement.Domain.Entities;

public class Rol
{
    public int RolID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    // Navigation properties
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
