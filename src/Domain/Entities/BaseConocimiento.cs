namespace IncidentManagement.Domain.Entities;

public class BaseConocimiento
{
    public int SolucionID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Problema { get; set; } = string.Empty;
    public string Solucion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Palabras_Clave { get; set; }
    public int? IncidenteRelacionadoID { get; set; }
    public int UsuarioCreadorID { get; set; }
    public int? TiempoResolucion { get; set; }
    public int Efectividad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // Navigation properties
    public virtual Incidente? IncidenteRelacionado { get; set; }
    public virtual Usuario UsuarioCreador { get; set; } = null!;
}
