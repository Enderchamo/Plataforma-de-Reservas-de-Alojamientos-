using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Dominio.Excepciones;

namespace PlataformaReservas.Aplicacion.Services;

public class ResenaService : IResenaService
{
    private readonly IResenaRepository _resenaRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IValidator<CrearResenaDto> _validator;
     private readonly IUserContext _userContext;

    public ResenaService(
        IResenaRepository resenaRepository,
        IReservaRepository reservaRepository,
        IValidator<CrearResenaDto> validator,
        IUserContext userContext)
    {
        _resenaRepository = resenaRepository;
        _reservaRepository = reservaRepository;
        _validator = validator;
        _userContext = userContext;
    }



    

    public async Task<Resena> CrearResenaAsync(CrearResenaDto resenaDTO)
    {       
        var usuarioId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");

        var validacion = await _validator.ValidateAsync(resenaDTO);
        if (!validacion.IsValid)
        {
            throw new ValidationException(validacion.Errors);
        }

        var reserva = await _reservaRepository.ObtenerPorIdAsync(resenaDTO.ReservaId);
        if (reserva == null)
        {
            throw new AppException("Reserva no encontrada.", 400, "ERROR_NEGOCIO");
        }

        if (reserva.UsuarioInvitadoId != usuarioId)
        {
            throw new AppException("Solo el huésped de la reserva puede dejar una reseña.", 403, "ACCESO_DENEGADO");
        }

        if (reserva.Estado != Reserva.EstadoEnum.Completada)
        {
            throw new AppException("La reserva debe estar completada para poder dejar una reseña.", 400, "ERROR_NEGOCIO");
        }

        bool yaTieneResena = await _resenaRepository.ExisteResenaPorReservaAsync(resenaDTO.ReservaId);
        if (yaTieneResena)
        {
            throw new AppException("Ya has dejado una reseña para esta reserva.", 400, "ERROR_NEGOCIO");
        }

        var nuevaResena = new Resena(resenaDTO.ReservaId, resenaDTO.Calificacion, resenaDTO.Comentario);
        await _resenaRepository.AgregarAsync(nuevaResena);

        return nuevaResena;
    }

    public async Task<IEnumerable<Resena>> ObtenerPorPropiedadAsync(int propiedadId)
    {
        return await _resenaRepository.ObtenerPorPropiedadIdAsync(propiedadId);
    }

    public async Task<ResumenResenasDto> ObtenerResumenPorPropiedadAsync(int propiedadId)
    {
        var resenas = await _resenaRepository.ObtenerPorPropiedadIdAsync(propiedadId);
        double promedio = resenas.Any() ? resenas.Average(r => r.Calificacion) : 0;

        return new ResumenResenasDto
        {
            PromedioCalificacion = Math.Round(promedio, 1),
            TotalResenas = resenas.Count(),
            Resenas = resenas
        };
    }
}