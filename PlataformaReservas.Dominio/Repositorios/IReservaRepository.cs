using System;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio.Repositorios;

public interface IReservaRepository
{
        Task<Reserva?> ObtenerPorIdAsync(int id);

        Task AgregarAsync(Reserva reserva);

        Task ActualizarAsync(Reserva reserva);

        
        Task<bool> VerificarDisponibilidadAsync(int propiedadId, DateTime fechaEntrada, DateTime fechaSalida);

        Task<IEnumerable<Reserva>> ObtenerPorUsuarioIdAsync(int usuarioId);
}
