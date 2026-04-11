using System;
using System.Threading.Tasks;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IResenaService
{
    Task<Resena> CrearResenaAsync(CrearResenaDto resenaDTO , int usuarioId);
    Task<IEnumerable<Resena>> ObtenerPorPropiedadAsync(int propiedadId);
    
}
