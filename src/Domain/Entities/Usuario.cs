namespace IncidentManagement.Domain.Entities;

public class Usuario
{
    public int UsuarioID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
    public int RolID { get; set; }
    public int? FacultadID { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
    public string? Telefono { get; set; }
    public string? Departamento { get; set; }
    public bool EsAsignador { get; set; }

    // Navigation properties
    public virtual Rol Rol { get; set; } = null!;
    public virtual Facultad? Facultad { get; set; }
    public virtual ICollection<Incidente> IncidentesReportados { get; set; } = new List<Incidente>();
    public virtual ICollection<AsignacionIncidente> Asignaciones { get; set; } = new List<AsignacionIncidente>();
    public virtual ICollection<ActualizacionIncidente> Actualizaciones { get; set; } = new List<ActualizacionIncidente>();
    public virtual ICollection<BaseConocimiento> ConocimientosCreados { get; set; } = new List<BaseConocimiento>();
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public virtual ICollection<SuscripcionPush> SuscripcionesPush { get; set; } = new List<SuscripcionPush>();
    public virtual ICollection<CatalogoServicio> ServiciosResponsable { get; set; } = new List<CatalogoServicio>();
}
