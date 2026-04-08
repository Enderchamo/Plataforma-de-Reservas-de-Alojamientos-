using System;
using Microsoft.EntityFrameworkCore;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;

namespace PlataformaReservas.Infraestructura.Repositorios;

public class PropiedadRepository : IPropiedadRepository
{
    private readonly ApplicationDbContext _context;

    public PropiedadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Propiedad?> ObtenerPorIdAsync(int id)
    {
        return await _context.Propiedades.FindAsync(id);
    }

    public async Task AgregarAsync(Propiedad propiedad)
    {
        await _context.Propiedades.AddAsync(propiedad);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Propiedad propiedad)
    {
        _context.Propiedades.Update(propiedad);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Propiedad propiedad)
    {
        _context.Propiedades.Remove(propiedad);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Propiedad>> BusquedaPorFiltroAsync(string? ubicacion, decimal? precioPorNoche, int? capacidad, DateTime? fechaEntrada, DateTime? fechaSalida)
    {
        var query = _context.Propiedades.AsQueryable();

        if (!string.IsNullOrWhiteSpace(ubicacion))
        {
            query = query.Where(p => p.Ubicacion.Contains(ubicacion));
        }

        if (precioPorNoche.HasValue)
        {
            query = query.Where(p => p.PrecioPorNoche <= precioPorNoche.Value);
        }

        if (capacidad.HasValue)
        {
            query = query.Where(p => p.Capacidad >= capacidad.Value);
        }

        if (fechaEntrada.HasValue && fechaSalida.HasValue)
        {
            query = query.Where(p => 
            
                !_context.Reservas.Any(r => 
                    r.PropiedadId == p.Id && 
                    r.Estado != Reserva.EstadoEnum.Cancelada && // Ignoramos las canceladas
                    r.FechaEntrada < fechaSalida.Value && 
                    r.FechaSalida > fechaEntrada.Value
                ) 
                &&
        
                !_context.FechasBloqueadas.Any(f => 
                    f.PropiedadId == p.Id && 
                    f.FechaInicio < fechaSalida.Value && 
                    f.FechaFin > fechaEntrada.Value
                )
            );
        }

        return await query.ToListAsync();
    }
}
