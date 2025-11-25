namespace IncidentManagement.Application.DTOs;

public class NotificacionDto
{
    public int NotificacionID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Mensaje { get; set; }
    public string? Tipo { get; set; }
    public bool Leida { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? URLAccion { get; set; }
}
