using System;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;


namespace PlataformaReservas.Infraestructura.Repositorios;

public class FechaBloqueadaRepository : IFechaBloqueadaRepository

{

    private readonly ApplicationDbContext _context;

    public FechaBloqueadaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(FechaBloqueada fechaBloqueada)
    {
        await _context.FechasBloqueadas.AddAsync(fechaBloqueada);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(FechaBloqueada fechaBloqueada)
    {
        _context.FechasBloqueadas.Remove(fechaBloqueada);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<FechaBloqueada>> ObtenerPorPropiedadIdAsync(int propiedadId)
    {
        return await _context.FechasBloqueadas
            .Where(f => f.PropiedadId == propiedadId)
            .ToListAsync();
    }
}
