namespace IncidentManagement.Domain.Entities;

public class SuscripcionPush
{
    public int SuscripcionID { get; set; }
    public int UsuarioID { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string? P256DH { get; set; }
    public string? Auth { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual Usuario Usuario { get; set; } = null!;
}
