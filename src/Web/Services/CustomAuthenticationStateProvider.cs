using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using IncidentManagement.Application.DTOs;
using System.Text.Json;

namespace IncidentManagement.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<string>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            
            if (string.IsNullOrWhiteSpace(userSession))
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var usuario = JsonSerializer.Deserialize<UsuarioDto>(userSession);
            if (usuario == null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
                new Claim("FacultadID", usuario.FacultadID?.ToString() ?? ""),
                new Claim("EsAsignador", usuario.EsAsignador.ToString())
            }, "CustomAuth"));

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(UsuarioDto? usuario)
    {
        ClaimsPrincipal claimsPrincipal;

        if (usuario != null)
        {
            await _sessionStorage.SetAsync("UserSession", JsonSerializer.Serialize(usuario));
            
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
            await _sessionStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task<UsuarioDto?> GetCurrentUserAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<string>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            
            if (string.IsNullOrWhiteSpace(userSession))
                return null;

            return JsonSerializer.Deserialize<UsuarioDto>(userSession);
        }
        catch
        {
            return null;
        }
    }
}
