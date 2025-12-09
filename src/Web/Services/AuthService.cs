using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Web.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _usuarioRepository = usuarioRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool Success, UsuarioDto? User, string Message)> LoginAsync(string email, string password)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null)
            {
                return (false, null, "Email no encontrado");
            }

            Console.WriteLine($"[AuthService] Usuario encontrado: {usuario.Email}, EsAsignador DB: {usuario.EsAsignador} (tipo: {usuario.EsAsignador.GetType()})");

            if (usuario.Contraseña != password)
            {
                return (false, null, "Contraseña incorrecta");
            }

            if (!usuario.Activo)
            {
                return (false, null, "Usuario inactivo");
            }

            var usuarioDto = new UsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                RolID = usuario.RolID,
                RolNombre = usuario.Rol?.Nombre ?? "Sin Rol",
                FacultadID = usuario.FacultadID,
                FacultadNombre = usuario.Facultad?.Nombre ?? "Sin Facultad",
                EsAsignador = usuario.EsAsignador,
                Activo = usuario.Activo
            };

            // Crear claims para la cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Usuario"),
                new Claim("EsAsignador", usuario.EsAsignador ? "True" : "False")
            };

            Console.WriteLine($"[AuthService] Login: {usuario.Email}, EsAsignador: {usuario.EsAsignador}, Claim: {(usuario.EsAsignador ? "True" : "False")}, FacultadID: {usuario.FacultadID}");

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // Sign in con cookie
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });
            }

            Console.WriteLine($"[AuthService] Login exitoso: {usuario.Email}, Rol: {usuario.Rol?.Nombre}");

            return (true, usuarioDto, "Login exitoso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Error en login: {ex.Message}");
            return (false, null, $"Error: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync("MyCookieAuth");
        }
        Console.WriteLine("[AuthService] Logout exitoso");
    }
}
