using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using IncidentManagement.Application.DTOs;

namespace IncidentManagement.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private UsuarioDto? _currentUserDto;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public Task UpdateAuthenticationState(UsuarioDto? usuario)
    {
        ClaimsPrincipal claimsPrincipal;

        if (usuario != null)
        {
            _currentUserDto = usuario;
            
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
                new Claim("FacultadID", usuario.FacultadID?.ToString() ?? ""),
                new Claim("EsAsignador", usuario.EsAsignador.ToString())
            }, "CustomAuth"));
        }
        else
        {
            _currentUserDto = null;
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        }

        _currentUser = claimsPrincipal;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        
        return Task.CompletedTask;
    }

    public Task<UsuarioDto?> GetCurrentUserAsync()
    {
        return Task.FromResult(_currentUserDto);
    }
}
