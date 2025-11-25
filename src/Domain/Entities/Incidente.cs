namespace IncidentManagement.Domain.Entities;

public class Incidente
{
    public int IncidenteID { get; set; }
    public string CodigoIncidente { get; set; } = string.Empty;
    public int ComputadoraID { get; set; }
    public int UsuarioReportadorID { get; set; }
    public int? FacultadID { get; set; }
    public int? LaboratorioID { get; set; }
    public int? ServicioID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = "Media";
    public string Estado { get; set; } = "Abierto";
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public bool Eliminado { get; set; }

    // Navigation properties
    public virtual Computadora Computadora { get; set; } = null!;
    public virtual Usuario UsuarioReportador { get; set; } = null!;
    public virtual Facultad? Facultad { get; set; }
    public virtual Laboratorio? Laboratorio { get; set; }
    public virtual CatalogoServicio? Servicio { get; set; }
    public virtual ICollection<AsignacionIncidente> Asignaciones { get; set; } = new List<AsignacionIncidente>();
    public virtual ICollection<ActualizacionIncidente> Actualizaciones { get; set; } = new List<ActualizacionIncidente>();
    public virtual ICollection<BaseConocimiento> Conocimientos { get; set; } = new List<BaseConocimiento>();
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
