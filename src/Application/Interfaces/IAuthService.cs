using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<UsuarioDto?> GetCurrentUserAsync();
}
