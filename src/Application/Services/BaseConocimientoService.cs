using IncidentManagement.Application.DTOs;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Domain.Entities;

namespace IncidentManagement.Application.Services;

public class BaseConocimientoService : IBaseConocimientoService
{
    private readonly IBaseConocimientoRepository _repository;

    public BaseConocimientoService(IBaseConocimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BaseConocimientoDto>> GetAllAsync()
    {
        var conocimientos = await _repository.GetAllAsync();
        return conocimientos.Select(MapToDto);
    }

    public async Task<IEnumerable<BaseConocimientoDto>> SearchAsync(string searchTerm)
    {
        var conocimientos = await _repository.SearchAsync(searchTerm);
        return conocimientos.Select(MapToDto);
    }

    public async Task<BaseConocimientoDto?> GetByIdAsync(int id)
    {
        var conocimiento = await _repository.GetByIdAsync(id);
        return conocimiento != null ? MapToDto(conocimiento) : null;
    }

    public async Task<BaseConocimientoDto> CreateAsync(CreateBaseConocimientoDto dto, int usuarioCreadorId)
    {
        var conocimiento = new BaseConocimiento
        {
            Titulo = dto.Titulo,
            Problema = dto.Problema,
            Solucion = dto.Solucion,
            Categoria = dto.Categoria,
            Palabras_Clave = dto.PalabrasClave,
            IncidenteRelacionadoID = dto.IncidenteRelacionadoID,
            UsuarioCreadorID = usuarioCreadorId,
            FechaCreacion = DateTime.Now,
            FechaActualizacion = DateTime.Now,
            Efectividad = 0
        };

        var creado = await _repository.CreateAsync(conocimiento);
        return MapToDto(creado);
    }

    private BaseConocimientoDto MapToDto(BaseConocimiento conocimiento)
    {
        return new BaseConocimientoDto
        {
            SolucionID = conocimiento.SolucionID,
            Titulo = conocimiento.Titulo,
            Problema = conocimiento.Problema,
            Solucion = conocimiento.Solucion,
            Categoria = conocimiento.Categoria,
            PalabrasClave = conocimiento.Palabras_Clave,
            FechaCreacion = conocimiento.FechaCreacion,
            Efectividad = conocimiento.Efectividad
        };
    }
}
