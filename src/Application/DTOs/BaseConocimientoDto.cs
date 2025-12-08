namespace IncidentManagement.Application.DTOs;

public class BaseConocimientoDto
{
    public int SolucionID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Problema { get; set; } = string.Empty;
    public string Solucion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? PalabrasClave { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int Efectividad { get; set; }
}

public class CreateBaseConocimientoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Problema { get; set; } = string.Empty;
    public string Solucion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? PalabrasClave { get; set; }
    public string? PalabrasClaveStr { get; set; }
    public int? IncidenteRelacionadoID { get; set; }
}
