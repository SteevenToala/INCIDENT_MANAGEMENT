using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class NotificacionRepository : INotificacionRepository
{
    private readonly ApplicationDbContext _context;

    public NotificacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notificacion?> GetByIdAsync(int id)
    {
        return await _context.Notificaciones
            .Include(n => n.Incidente)
            .FirstOrDefaultAsync(n => n.NotificacionID == id);
    }

    public async Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .Include(n => n.Incidente)
            .Where(n => n.UsuarioID == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetNoLeidasAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .Include(n => n.Incidente)
            .Where(n => n.UsuarioID == usuarioId && !n.Leida)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Notificacion> CreateAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
        return notificacion;
    }

    public async Task UpdateAsync(Notificacion notificacion)
    {
        _context.Entry(notificacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task MarcarComoLeidaAsync(int notificacionId)
    {
        var notificacion = await _context.Notificaciones.FindAsync(notificacionId);
        if (notificacion != null)
        {
            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
