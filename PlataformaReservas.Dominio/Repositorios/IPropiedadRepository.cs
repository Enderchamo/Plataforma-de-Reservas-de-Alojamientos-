using System;
using System.Threading.Tasks;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio.Repositorios;

public interface IPropiedadRepository
{
    
    Task<Propiedad?> ObtenerPorIdAsync(int id);

    Task AgregarAsync(Propiedad propiedad);

    Task ActualizarAsync(Propiedad propiedad);

    Task EliminarAsync(Propiedad propiedad);

    Task<bool> ExistePropiedadPorTituloYHostAsync(string titulo, int hostId);

    Task<IEnumerable<Propiedad>> BusquedaPorFiltroAsync(string? ubicacion, decimal? precioPorNoche, int? capacidad,DateTime? fechaEntrada, DateTime? fechaSalida);



}
