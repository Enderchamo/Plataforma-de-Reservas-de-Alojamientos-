using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IPropiedadService
{
    // Escritura: El ID del Host se obtiene del token internamente
    Task<Propiedad> CrearPropiedadAsync(CrearPropiedadDto dto);
    Task ActualizarPropiedadAsync(int id, CrearPropiedadDto dto);
    Task EliminarPropiedadAsync(int id);
    Task ActualizarImagenAsync(int id, string rutaImagen);

    // Lectura
    Task<IEnumerable<Propiedad>> BuscarPropiedadesAsync(string? ubicacion, decimal? precioMaximo, int? capacidadMinimas, DateTime? fechaEntrada, DateTime? fechaSalida);
    Task<Propiedad?> ObtenerPorIdAsync(int id);
}