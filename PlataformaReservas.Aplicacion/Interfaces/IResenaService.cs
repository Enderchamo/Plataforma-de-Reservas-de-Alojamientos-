using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IResenaService
{
    Task<Resena> CrearResenaAsync(CrearResenaDto resenaDTO);
    Task<IEnumerable<Resena>> ObtenerPorPropiedadAsync(int propiedadId);
    Task<ResumenResenasDto> ObtenerResumenPorPropiedadAsync(int propiedadId); // El nuevo método
}