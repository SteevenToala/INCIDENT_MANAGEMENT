using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class BaseConocimientoRepository : IBaseConocimientoRepository
{
    private readonly ApplicationDbContext _context;

    public BaseConocimientoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseConocimiento?> GetByIdAsync(int id)
    {
        return await _context.BaseConocimientos
            .Include(b => b.UsuarioCreador)
            .Include(b => b.IncidenteRelacionado)
            .FirstOrDefaultAsync(b => b.SolucionID == id);
    }

    public async Task<IEnumerable<BaseConocimiento>> GetAllAsync()
    {
        return await _context.BaseConocimientos
            .Include(b => b.UsuarioCreador)
            .OrderByDescending(b => b.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<BaseConocimiento>> SearchAsync(string searchTerm)
    {
        return await _context.BaseConocimientos
            .Include(b => b.UsuarioCreador)
            .Where(b => 
                b.Titulo.Contains(searchTerm) ||
                b.Problema.Contains(searchTerm) ||
                b.Solucion.Contains(searchTerm) ||
                (b.Palabras_Clave != null && b.Palabras_Clave.Contains(searchTerm)))
            .OrderByDescending(b => b.Efectividad)
            .ToListAsync();
    }

    public async Task<IEnumerable<BaseConocimiento>> GetByCategoriaAsync(string categoria)
    {
        return await _context.BaseConocimientos
            .Include(b => b.UsuarioCreador)
            .Where(b => b.Categoria == categoria)
            .ToListAsync();
    }

    public async Task<BaseConocimiento> CreateAsync(BaseConocimiento conocimiento)
    {
        _context.BaseConocimientos.Add(conocimiento);
        await _context.SaveChangesAsync();
        return conocimiento;
    }

    public async Task UpdateAsync(BaseConocimiento conocimiento)
    {
        _context.Entry(conocimiento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var conocimiento = await _context.BaseConocimientos.FindAsync(id);
        if (conocimiento != null)
        {
            _context.BaseConocimientos.Remove(conocimiento);
            await _context.SaveChangesAsync();
        }
    }
}
