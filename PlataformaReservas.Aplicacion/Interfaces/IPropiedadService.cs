using System;
using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;


namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IPropiedadService
{
    Task<Propiedad> CrearPropiedadAsync(CrearPropiedadDto dto);
    Task ActualizarPropiedadAsync(int id, CrearPropiedadDto dto, int usuarioId);
    Task EliminarPropiedadAsync(int id, int usuarioId);
    Task<IEnumerable<Propiedad>> BuscarPropiedadesAsync(string? ubicacion, decimal? prcioMaximo, int? capacidadMinimas, DateTime? fechaEntrada, DateTime? fechaSalida );
    Task<Propiedad?> ObtenerPorIdAsync(int id);
    
}
