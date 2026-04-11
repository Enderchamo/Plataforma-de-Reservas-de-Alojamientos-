using System;
using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IFechaBloqueadaService
{
    Task<FechaBloqueada> BloquearFechaAsync(BloquearFechaDto dto, int hostId);
    Task<IEnumerable<FechaBloqueada>> ObtenerPorPropiedadAsync(int propiedadId);
}
