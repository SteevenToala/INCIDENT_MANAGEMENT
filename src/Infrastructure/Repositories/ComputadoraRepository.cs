using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class ComputadoraRepository : IComputadoraRepository
{
    private readonly ApplicationDbContext _context;

    public ComputadoraRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Computadora>> GetAllAsync()
    {
        return await _context.Computadoras
            .Include(c => c.Laboratorio)
                .ThenInclude(l => l.Facultad)
            .ToListAsync();
    }

    public async Task<Computadora?> GetByIdAsync(int id)
    {
        return await _context.Computadoras
            .Include(c => c.Laboratorio)
                .ThenInclude(l => l.Facultad)
            .FirstOrDefaultAsync(c => c.ComputadoraID == id);
    }

    public async Task<IEnumerable<Computadora>> GetByLaboratorioAsync(int laboratorioId)
    {
        return await _context.Computadoras
            .Include(c => c.Laboratorio)
            .Where(c => c.LaboratorioID == laboratorioId)
            .ToListAsync();
    }

    public async Task<Computadora?> GetByCodigoAsync(string codigo)
    {
        return await _context.Computadoras
            .Include(c => c.Laboratorio)
                .ThenInclude(l => l.Facultad)
            .FirstOrDefaultAsync(c => c.CodigoEquipo == codigo);
    }
}
