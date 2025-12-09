using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class CatalogoServicioRepository : ICatalogoServicioRepository
{
    private readonly ApplicationDbContext _context;

    public CatalogoServicioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CatalogoServicio>> GetAllAsync()
    {
        return await _context.CatalogoServicios
            .Where(s => s.Estado == true)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<CatalogoServicio?> GetByIdAsync(int id)
    {
        return await _context.CatalogoServicios.FindAsync(id);
    }
}
