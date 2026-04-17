using System;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IReservaService
{
    Task<Reserva> CrearReservaAsync(CrearReservaDto dto);
    Task CancelarReservaAsync(int reservaId);
    Task CompletarReservaAsync(int reservaId);
    Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync();
}
