using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class SuscripcionPushRepository : ISuscripcionPushRepository
{
    private readonly ApplicationDbContext _context;

    public SuscripcionPushRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuscripcionPush?> GetByIdAsync(int id)
    {
        return await _context.SuscripcionesPush
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.SuscripcionID == id);
    }

    public async Task<IEnumerable<SuscripcionPush>> GetByUsuarioAsync(int usuarioId)
    {
        return await _context.SuscripcionesPush
            .Where(s => s.UsuarioID == usuarioId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SuscripcionPush>> GetActivasByUsuarioAsync(int usuarioId)
    {
        return await _context.SuscripcionesPush
            .Where(s => s.UsuarioID == usuarioId && s.Activa)
            .ToListAsync();
    }

    public async Task<SuscripcionPush> CreateAsync(SuscripcionPush suscripcion)
    {
        _context.SuscripcionesPush.Add(suscripcion);
        await _context.SaveChangesAsync();
        return suscripcion;
    }

    public async Task UpdateAsync(SuscripcionPush suscripcion)
    {
        _context.Entry(suscripcion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var suscripcion = await _context.SuscripcionesPush.FindAsync(id);
        if (suscripcion != null)
        {
            _context.SuscripcionesPush.Remove(suscripcion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SuscripcionPush?> GetByEndpointAsync(string endpoint)
    {
        return await _context.SuscripcionesPush
            .FirstOrDefaultAsync(s => s.Endpoint == endpoint);
    }
}
