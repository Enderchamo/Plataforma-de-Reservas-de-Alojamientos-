using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Excepciones;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IValidator<CrearReservaDto> _crearReservaValidator;
    private readonly IUserContext _userContext;
    private readonly INotificacionFacade _notificacionFacade;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReservaService(
        IReservaRepository reservaRepository,
        IPropiedadRepository propiedadRepository,
        IValidator<CrearReservaDto> crearReservaValidator,
        IUserContext userContext,
        INotificacionFacade notificacionFacade,
        IUsuarioRepository usuarioRepository)
    {
        _reservaRepository = reservaRepository;
        _propiedadRepository = propiedadRepository;
        _crearReservaValidator = crearReservaValidator;
        _userContext = userContext;
        _notificacionFacade = notificacionFacade;
        _usuarioRepository = usuarioRepository;
    }

    public async Task CancelarReservaAsync(int reservaId)
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");

        var reserva = await _reservaRepository.ObtenerPorIdAsync(reservaId);
        if (reserva == null)
            throw new AppException("Reserva no encontrada.", 404, "NO_ENCONTRADA");

        if (reserva.UsuarioInvitadoId != usuarioId)
            throw new AppException("No tienes permiso para cancelar esta reserva.", 403, "ACCESO_DENEGADO");

        reserva.CancelarEstado(DateTime.UtcNow);
        await _reservaRepository.ActualizarAsync(reserva);

        // Notificaciones Asíncronas
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(reserva.PropiedadId);
        var host = await _usuarioRepository.ObtenerPorIdAsync(propiedad!.HostId);
        var guest = await _usuarioRepository.ObtenerPorIdAsync(reserva.UsuarioInvitadoId);

        if (host != null)
            await _notificacionFacade.EnviarNotificacionYCorreoAsync(host.Id, host.Email, "Reserva Cancelada", $"La reserva para '{propiedad.Titulo}' ha sido cancelada.");
        
        if (guest != null)
            await _notificacionFacade.EnviarNotificacionYCorreoAsync(guest.Id, guest.Email, "Cancelación Exitosa", $"Has cancelado tu reserva en '{propiedad.Titulo}'.");
    }

    public async Task CompletarReservaAsync(int reservaId)
    {
        var reserva = await _reservaRepository.ObtenerPorIdAsync(reservaId);
        if (reserva == null)
            throw new AppException("Reserva no encontrada.", 404, "NO_ENCONTRADA");

        reserva.CompletarEstado();
        await _reservaRepository.ActualizarAsync(reserva);

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(reserva.PropiedadId);
        var guest = await _usuarioRepository.ObtenerPorIdAsync(reserva.UsuarioInvitadoId);

        if (guest != null)
        {
            await _notificacionFacade.EnviarNotificacionYCorreoAsync(guest.Id, guest.Email, "Estancia Finalizada", 
                $"Tu estancia en '{propiedad!.Titulo}' ha finalizado. ¡Ya puedes dejar una reseña!");
        }
    }

    public async Task<IEnumerable<ReservaRecibidaDto>> ObtenerReservasRecibidasAsync()
    {
        var hostId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        
        // 1. Obtenemos las entidades con sus Includes desde el Repositorio
        var reservas = await _reservaRepository.ObtenerReservasPorHostIdAsync(hostId);

        // 2. Las transformamos en DTOs manualmente
        var dtos = reservas.Select(r => new ReservaRecibidaDto
        {
            Id = r.Id,
            FechaEntrada = r.FechaEntrada,
            FechaSalida = r.FechaSalida,
            Estado = r.Estado.ToString(),
            PropiedadId = r.PropiedadId,
            PropiedadTitulo = r.Propiedad.Titulo,
            HuespedId = r.UsuarioInvitadoId,
            HuespedNombre = r.UsuarioInvitado.Nombre,
            HuespedEmail = r.UsuarioInvitado.Email
        });

        return dtos;
    }

    public async Task<Reserva> CrearReservaAsync(CrearReservaDto dto)
    {
        var validacion = await _crearReservaValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(dto.PropiedadId);
        if (propiedad == null)
            throw new AppException("Propiedad no encontrada.", 404, "NO_ENCONTRADA");

        if (propiedad.HostId == dto.UsuarioInvitadoId)
            throw new AppException("No puedes reservar tu propia propiedad.", 400, "OPERACION_INVALIDA");

        bool estaDisponible = await _reservaRepository.VerificarDisponibilidadAsync(dto.PropiedadId, dto.FechaEntrada, dto.FechaSalida);
        if (!estaDisponible)
            throw new AppException("La propiedad no está disponible para estas fechas.", 400, "FECHAS_OCUPADAS");

        var nuevaReserva = new Reserva(dto.PropiedadId, dto.UsuarioInvitadoId, dto.FechaEntrada, dto.FechaSalida);
        await _reservaRepository.CrearReservaSeguraAsync(nuevaReserva);

        // Notificación al Host
        var host = await _usuarioRepository.ObtenerPorIdAsync(propiedad.HostId);
        if (host != null)
        {
            await _notificacionFacade.EnviarNotificacionYCorreoAsync(host.Id, host.Email, "Nueva Reserva Recibida", 
                $"¡Tienes una nueva reserva para '{propiedad.Titulo}'!");
        }

        return nuevaReserva;
    }

    public async Task<IEnumerable<Reserva>> ObtenerReservasPorUsuarioAsync()
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        return await _reservaRepository.ObtenerPorUsuarioIdAsync(usuarioId);
    }
}