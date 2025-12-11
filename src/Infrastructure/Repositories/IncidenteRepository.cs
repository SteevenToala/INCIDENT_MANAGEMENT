using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class IncidenteRepository : IIncidenteRepository
{
    private readonly ApplicationDbContext _context;

    public IncidenteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Incidente?> GetByIdAsync(int id)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
                .ThenInclude(c => c.Laboratorio)
            .Include(i => i.UsuarioReportador)
            .Include(i => i.Laboratorio)
            .Include(i => i.Facultad)
            .Include(i => i.Servicio)
            .Include(i => i.Asignaciones)
            .FirstOrDefaultAsync(i => i.IncidenteID == id && !i.Eliminado);
    }

    public async Task<Incidente?> GetByCodigoAsync(string codigo)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
            .Include(i => i.UsuarioReportador)
            .FirstOrDefaultAsync(i => i.CodigoIncidente == codigo && !i.Eliminado);
    }

    public async Task<IEnumerable<Incidente>> GetAllAsync()
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
                .ThenInclude(c => c.Laboratorio)
            .Include(i => i.UsuarioReportador)
            .Include(i => i.Laboratorio)
            .Include(i => i.Asignaciones)
            .Where(i => !i.Eliminado)
            .OrderByDescending(i => i.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetByUsuarioAsync(int usuarioId)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
                .ThenInclude(c => c.Laboratorio)
            .Include(i => i.UsuarioReportador)
            .Include(i => i.Laboratorio)
            .Include(i => i.Asignaciones)
            .Where(i => i.UsuarioReportadorID == usuarioId && !i.Eliminado)
            .OrderByDescending(i => i.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetByEstadoAsync(string estado)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
            .Include(i => i.UsuarioReportador)
            .Where(i => i.Estado == estado && !i.Eliminado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetByLaboratorioAsync(int laboratorioId)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
            .Include(i => i.UsuarioReportador)
            .Where(i => i.LaboratorioID == laboratorioId && !i.Eliminado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetByFacultadAsync(int facultadId)
    {
        return await _context.Incidentes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Computadora)
                .ThenInclude(c => c.Laboratorio)
            .Include(i => i.UsuarioReportador)
                .ThenInclude(u => u.Rol)
            .Include(i => i.Laboratorio)
            .Include(i => i.Facultad)
            .Include(i => i.Asignaciones)
                .ThenInclude(a => a.UsuarioAsignado)
            .Where(i => !i.Eliminado && (
                i.FacultadID == facultadId || 
                (i.Computadora != null && i.Computadora.Laboratorio != null && i.Computadora.Laboratorio.FacultadID == facultadId)
            ))
            .OrderByDescending(i => i.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Incidente> CreateAsync(Incidente incidente)
    {
        _context.Incidentes.Add(incidente);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(incidente.IncidenteID))!;
    }

    public async Task UpdateAsync(Incidente incidente)
    {
        // Buscar la entidad existente en el contexto
        var existingIncidente = await _context.Incidentes.FindAsync(incidente.IncidenteID);
        
        if (existingIncidente == null)
            throw new Exception("Incidente no encontrado");

        // Actualizar las propiedades necesarias
        existingIncidente.Estado = incidente.Estado;
        existingIncidente.Prioridad = incidente.Prioridad;
        existingIncidente.Descripcion = incidente.Descripcion;
        existingIncidente.FechaActualizacion = incidente.FechaActualizacion;
        existingIncidente.FechaResolucion = incidente.FechaResolucion;
        existingIncidente.Eliminado = incidente.Eliminado;
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var incidente = await _context.Incidentes.FindAsync(id);
        if (incidente != null)
        {
            incidente.Eliminado = true;
            await _context.SaveChangesAsync();
        }
    }
}
