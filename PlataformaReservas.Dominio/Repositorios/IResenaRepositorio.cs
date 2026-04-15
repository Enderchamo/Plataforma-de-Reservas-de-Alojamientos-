using System;
using PlataformaReservas.Dominio.Entidades;



namespace PlataformaReservas.Dominio.Repositorios;

public interface IResenaRepository
{

    Task<IEnumerable<Resena>> ObtenerPorPropiedadIdAsync(int propiedadId);
    Task AgregarAsync(Resena resena);

    Task<bool> ExisteResenaPorReservaAsync(int reservaId);
}
