using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Application.DTOs;
using System.Security.Claims;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;
    private readonly ISuscripcionPushRepository _suscripcionRepository;

    public NotificacionesController(
        INotificacionService notificacionService,
        ISuscripcionPushRepository suscripcionRepository)
    {
        _notificacionService = notificacionService;
        _suscripcionRepository = suscripcionRepository;
    }

    private int GetUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    // GET: api/notificaciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetNotificaciones()
    {
        var usuarioId = GetUsuarioId();
        var notificaciones = await _notificacionService.GetNotificacionesByUsuarioAsync(usuarioId);
        return Ok(notificaciones);
    }

    // GET: api/notificaciones/no-leidas
    [HttpGet("no-leidas")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetNotificacionesNoLeidas()
    {
        var usuarioId = GetUsuarioId();
        var notificaciones = await _notificacionService.GetNotificacionesNoLeidasAsync(usuarioId);
        return Ok(notificaciones);
    }

    // GET: api/notificaciones/contador
    [HttpGet("contador")]
    public async Task<ActionResult<int>> GetContadorNoLeidas()
    {
        var usuarioId = GetUsuarioId();
        var contador = await _notificacionService.GetContadorNoLeidasAsync(usuarioId);
        return Ok(contador);
    }

    // PUT: api/notificaciones/{id}/marcar-leida
    [HttpPut("{id}/marcar-leida")]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        await _notificacionService.MarcarComoLeidaAsync(id);
        return NoContent();
    }

    // PUT: api/notificaciones/marcar-todas-leidas
    [HttpPut("marcar-todas-leidas")]
    public async Task<IActionResult> MarcarTodasComoLeidas()
    {
        var usuarioId = GetUsuarioId();
        await _notificacionService.MarcarTodasComoLeidasAsync(usuarioId);
        return NoContent();
    }

    // POST: api/notificaciones/suscribir
    [HttpPost("suscribir")]
    public async Task<IActionResult> Suscribir([FromBody] SuscripcionPushDto suscripcionDto)
    {
        var usuarioId = GetUsuarioId();

        // Verificar si ya existe una suscripción con este endpoint
        var existente = await _suscripcionRepository.GetByEndpointAsync(suscripcionDto.Endpoint);
        if (existente != null)
        {
            // Actualizar la suscripción existente
            existente.P256DH = suscripcionDto.P256DH;
            existente.Auth = suscripcionDto.Auth;
            existente.Activa = true;
            await _suscripcionRepository.UpdateAsync(existente);
            return Ok(new { message = "Suscripción actualizada" });
        }

        var suscripcion = new IncidentManagement.Domain.Entities.SuscripcionPush
        {
            UsuarioID = usuarioId,
            Endpoint = suscripcionDto.Endpoint,
            P256DH = suscripcionDto.P256DH,
            Auth = suscripcionDto.Auth,
            Activa = true,
            FechaCreacion = DateTime.Now
        };

        await _suscripcionRepository.CreateAsync(suscripcion);
        return Ok(new { message = "Suscripción creada exitosamente" });
    }

    // DELETE: api/notificaciones/desuscribir
    [HttpDelete("desuscribir")]
    public async Task<IActionResult> Desuscribir([FromBody] SuscripcionPushDto suscripcionDto)
    {
        var suscripcion = await _suscripcionRepository.GetByEndpointAsync(suscripcionDto.Endpoint);
        if (suscripcion != null)
        {
            await _suscripcionRepository.DeleteAsync(suscripcion.SuscripcionID);
        }
        return NoContent();
    }
}
