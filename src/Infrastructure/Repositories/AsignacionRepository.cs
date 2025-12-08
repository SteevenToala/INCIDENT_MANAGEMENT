using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.Infrastructure.Repositories;

public class AsignacionRepository : IAsignacionRepository
{
    private readonly ApplicationDbContext _context;

    public AsignacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AsignacionIncidente>> GetAllAsync()
    {
        return await _context.AsignacionesIncidentes
            .Include(a => a.UsuarioAsignado)
            .Include(a => a.Incidente)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AsignacionIncidente?> GetByIdAsync(int id)
    {
        return await _context.AsignacionesIncidentes
            .Include(a => a.UsuarioAsignado)
            .Include(a => a.Incidente)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AsignacionID == id);
    }

    public async Task<IEnumerable<AsignacionIncidente>> GetByIncidenteIdAsync(int incidenteId)
    {
        return await _context.AsignacionesIncidentes
            .Include(a => a.UsuarioAsignado)
            .Where(a => a.IncidenteID == incidenteId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AsignacionIncidente>> GetByUsuarioAsignadoIdAsync(int usuarioId)
    {
        return await _context.AsignacionesIncidentes
            .Include(a => a.Incidente)
            .Where(a => a.UsuarioAsignadoID == usuarioId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AsignacionIncidente> CreateAsync(AsignacionIncidente asignacion)
    {
        _context.AsignacionesIncidentes.Add(asignacion);
        await _context.SaveChangesAsync();
        return asignacion;
    }

    public async Task UpdateAsync(AsignacionIncidente asignacion)
    {
        var existing = await _context.AsignacionesIncidentes.FindAsync(asignacion.AsignacionID);
        if (existing != null)
        {
            existing.EstadoAsignacion = asignacion.EstadoAsignacion;
            existing.FechaAceptacion = asignacion.FechaAceptacion;
            existing.FechaCompletacion = asignacion.FechaCompletacion;
            existing.Notas = asignacion.Notas;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var asignacion = await _context.AsignacionesIncidentes.FindAsync(id);
        if (asignacion != null)
        {
            _context.AsignacionesIncidentes.Remove(asignacion);
            await _context.SaveChangesAsync();
        }
    }
}
