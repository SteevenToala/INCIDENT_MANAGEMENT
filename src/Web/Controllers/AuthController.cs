using IncidentManagement.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("auth")]
[IgnoreAntiforgeryToken]
public class AuthController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtOptions)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginFormRequest request)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null || usuario.Contraseña != request.Password || !usuario.Activo)
            {
                return Redirect($"/login?error={Uri.EscapeDataString("Credenciales inválidas")}");
            }

            var token = _tokenService.GenerateToken(usuario);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                MaxAge = TimeSpan.FromMinutes(_jwtSettings.ExpirationMinutes),
                IsEssential = true,
            });

            var redirectTarget = SanitizeReturnUrl(request.ReturnUrl) ?? GetHomeForRole(usuario.Rol?.Nombre);

            return Redirect(redirectTarget);
        }
        catch (Exception ex)
        {
            return Redirect($"/login?error={Uri.EscapeDataString(ex.Message)}");
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return Redirect("/login");
    }

    private static string? SanitizeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return null;
        }

        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out _))
        {
            return null;
        }

        return returnUrl.StartsWith('/') ? returnUrl : $"/{returnUrl.TrimStart('/')}";
    }

    private static string GetHomeForRole(string? roleName)
    {
        return (roleName ?? string.Empty).ToLower() switch
        {
            "estudiante" => "/estudiante/home",
            "docente" => "/docente/home",
            "administrativo" => "/administrativo/home",
            "laboratorista" => "/laboratorista/home",
            _ => "/"
        };
    }
}

public class LoginFormRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
