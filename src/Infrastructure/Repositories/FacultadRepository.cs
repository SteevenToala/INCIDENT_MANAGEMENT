using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class FacultadRepository : IFacultadRepository
{
    private readonly ApplicationDbContext _context;

    public FacultadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Facultad>> GetAllAsync()
    {
        return await _context.Facultades.ToListAsync();
    }

    public async Task<Facultad?> GetByIdAsync(int id)
    {
        return await _context.Facultades.FindAsync(id);
    }
}
