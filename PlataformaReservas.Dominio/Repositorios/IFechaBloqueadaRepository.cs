using System;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio.Repositorios;

public interface IFechaBloqueadaRepository
{
    
    Task AgregarAsync(FechaBloqueada fechaBloqueada);
    Task EliminarAsync(FechaBloqueada fechaBloqueada);
    Task<IEnumerable<FechaBloqueada>> ObtenerPorPropiedadIdAsync(int propiedadId);
}
