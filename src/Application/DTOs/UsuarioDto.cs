namespace IncidentManagement.Application.DTOs;

public class UsuarioDto
{
    public int UsuarioID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public int RolID { get; set; }
    public string? FacultadNombre { get; set; }
    public int? FacultadID { get; set; }
    public bool EsAsignador { get; set; }
    public bool Activo { get; set; }
}
