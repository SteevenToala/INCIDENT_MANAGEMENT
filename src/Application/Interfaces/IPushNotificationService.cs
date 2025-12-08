using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Interfaces;

public interface IPushNotificationService
{
    Task<bool> SendNotificationAsync(int usuarioId, string titulo, string mensaje, string? urlAccion = null);
    Task<bool> SendNotificationToMultipleUsersAsync(IEnumerable<int> usuarioIds, string titulo, string mensaje, string? urlAccion = null);
}
