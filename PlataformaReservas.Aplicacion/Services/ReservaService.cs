using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class ReservaService : IReservaService
{

    private readonly IReservaRepository _reservaRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IValidator<CrearReservaDto> _crearReservaValidator;

    public ReservaService(IReservaRepository reservaRepository,IPropiedadRepository propiedadRepository,INotificacionRepository notificacionRepository,IValidator<CrearReservaDto> crearReservaValidator)
    {
        _reservaRepository = reservaRepository;
        _propiedadRepository = propiedadRepository;
        _notificacionRepository = notificacionRepository;
        _crearReservaValidator = crearReservaValidator;
    }

    public async Task CancelarReservaAsync(int reservaId, int usuarioId)
    {
        var reserva = await _reservaRepository.ObtenerPorIdAsync(reservaId);
        if (reserva==null)

        {
            throw new InvalidOperationException(" Reserva no encontrada. ");
        }

        if (reserva.UsuarioInvitadoId != usuarioId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para cancelar esta reserva.");      
        }

        // Cambiar estado usando la lógica de negocio de la entidad.
        reserva.CancelarEstado(DateTime.UtcNow);
        await _reservaRepository.ActualizarAsync(reserva);

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(reserva.PropiedadId);

        await _notificacionRepository.AgregarAsync(new Notificacion($"La reserva de la propiedad '{propiedad!.Titulo}' ha sido cancelada.", propiedad.HostId)); //Para el Host.
        await _notificacionRepository.AgregarAsync(new Notificacion($"Has cancelado tu reserva en '{propiedad.Titulo}'.", reserva.UsuarioInvitadoId)); // Al Guest

    }

    public async Task CompletarReservaAsync(int reservaId, int usuarioId)
    {
        var reserva = await _reservaRepository.ObtenerPorIdAsync(reservaId);

        if (reserva == null)
        {
            throw new InvalidOperationException("Reserva no encontrada.");
        }

        // Cambiar estado
        reserva.CompletarEstado();
        await _reservaRepository.ActualizarAsync(reserva);

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(reserva.PropiedadId);

        await _notificacionRepository.AgregarAsync(new Notificacion($"Tu estancia en '{propiedad!.Titulo}' ha finalizado. ¡Esperamos que lo hayas disfrutado! Ya puedes dejar una reseña.", reserva.UsuarioInvitadoId));

    }

    public async Task<Reserva> CrearReservaAsync(CrearReservaDto dto)
    {
        var validacion = await _crearReservaValidator.ValidateAsync(dto);

        if(!validacion.IsValid)
        {
            throw new ValidationException(validacion.Errors);
        }

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(dto.PropiedadId);
        
        if (propiedad==null)
        {
            throw new InvalidOperationException("La propiedad no existe");
        }

        if (propiedad.HostId == dto.UsuarioInvitadoId)
        {
            throw new InvalidOperationException("No puedes reservar tu propia propiedad");

        }

        bool estaDisponible = await _reservaRepository.VerificarDisponibilidadAsync(dto.PropiedadId, dto.FechaEntrada,dto.FechaSalida);

        if(!estaDisponible)
        {
            throw new InvalidOperationException("La propiedad no esta disponible en las fechas seleccionadas.");

        }

        var nuevaReserva = new Reserva(dto.PropiedadId, dto.UsuarioInvitadoId, dto.FechaEntrada, dto.FechaSalida);

        await _reservaRepository.CrearReservaSeguraAsync(nuevaReserva);

        var mensaje = $"!Tienes una nueva reserva para '{propiedad.Titulo}' del {dto.FechaEntrada:dd/MM/yyyy} al {dto.FechaSalida:dd/MM/yyyy}!";
        await _notificacionRepository.AgregarAsync(new Notificacion(mensaje,propiedad.HostId));

        return nuevaReserva;
    }

    public async Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync(int usuarioId)
    {
        return await _reservaRepository.ObtenerPorUsuarioIdAsync(usuarioId);
    }

    
}
