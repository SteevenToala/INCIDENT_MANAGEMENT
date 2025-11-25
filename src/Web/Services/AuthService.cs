using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;

namespace IncidentManagement.Web.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        CustomAuthenticationStateProvider authStateProvider)
    {
        _usuarioRepository = usuarioRepository;
        _authStateProvider = authStateProvider;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "El correo electrónico no está registrado"
                };
            }

            if (usuario.Contraseña != request.Password)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La contraseña es incorrecta"
                };
            }

            if (!usuario.Activo)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tu cuenta está desactivada. Contacta al administrador"
                };
            }

            var usuarioDto = new UsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                RolNombre = usuario.Rol.Nombre,
                RolID = usuario.RolID,
                FacultadNombre = usuario.Facultad?.Nombre,
                FacultadID = usuario.FacultadID,
                EsAsignador = usuario.EsAsignador,
                Activo = usuario.Activo
            };

            await _authStateProvider.UpdateAuthenticationState(usuarioDto);

            return new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                Usuario = usuarioDto
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"Error al procesar el login: {ex.Message}"
            };
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.UpdateAuthenticationState(null);
    }

    public async Task<UsuarioDto?> GetCurrentUserAsync()
    {
        return await _authStateProvider.GetCurrentUserAsync();
    }
}
