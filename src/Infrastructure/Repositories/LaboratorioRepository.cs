using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class LaboratorioRepository : ILaboratorioRepository
{
    private readonly ApplicationDbContext _context;

    public LaboratorioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Laboratorio>> GetAllAsync()
    {
        return await _context.Laboratorios
            .Include(l => l.Facultad)
            .ToListAsync();
    }

    public async Task<Laboratorio?> GetByIdAsync(int id)
    {
        return await _context.Laboratorios
            .Include(l => l.Facultad)
            .FirstOrDefaultAsync(l => l.LaboratorioID == id);
    }

    public async Task<IEnumerable<Laboratorio>> GetByFacultadAsync(int facultadId)
    {
        return await _context.Laboratorios
            .Include(l => l.Facultad)
            .Where(l => l.FacultadID == facultadId)
            .ToListAsync();
    }
}
