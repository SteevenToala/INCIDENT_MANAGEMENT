using Microsoft.EntityFrameworkCore;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;

namespace IncidentManagement.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .AsSplitQuery()
            .Include(u => u.Rol)
            .Include(u => u.Facultad)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Facultad)
            .FirstOrDefaultAsync(u => u.UsuarioID == id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Facultad)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetByRolAsync(string rolNombre)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Facultad)
            .Where(u => u.Rol.Nombre == rolNombre)
            .ToListAsync();
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string contraseña)
    {
        var usuario = await GetByEmailAsync(email);
        return usuario != null && usuario.Contraseña == contraseña && usuario.Activo;
    }
}
