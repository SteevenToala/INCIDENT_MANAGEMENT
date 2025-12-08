namespace IncidentManagement.Application.DTOs;

public class SuscripcionPushDto
{
    public string Endpoint { get; set; } = string.Empty;
    public string? P256DH { get; set; }
    public string? Auth { get; set; }
}

public class PushNotificationPayloadDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? URLAccion { get; set; }
    public string? Icono { get; set; }
    public string? Tipo { get; set; }
}
