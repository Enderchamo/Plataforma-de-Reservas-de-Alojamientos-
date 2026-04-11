using System;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IReservaService
{
    Task<Reserva> CrearReservaAsync(CrearReservaDto dto);
    Task CancelarReservaAsync(int reservaId, int usuarioId);
    Task CompletarReservaAsync(int reservaId, int usuarioId);
    Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync(int usuarioId);
}
