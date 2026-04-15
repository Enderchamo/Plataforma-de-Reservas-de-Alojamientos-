using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class ResenaService : IResenaService
{

    private readonly IResenaRepository _resenaRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IValidator<CrearResenaDto> _validator;
    
    public ResenaService(IResenaRepository resenaRepository,IReservaRepository reservaRepository,IValidator<CrearResenaDto> validator)
    {
        _resenaRepository = resenaRepository;   
        _reservaRepository = reservaRepository;
        _validator = validator;
    }


    public async Task<Resena> CrearResenaAsync(CrearResenaDto resenaDTO, int usuarioId)
    {
        var validacion =await _validator.ValidateAsync(resenaDTO);
        if (!validacion.IsValid)
        {
            throw new ValidationException(validacion.Errors);
        }

        var reserva = await _reservaRepository.ObtenerPorIdAsync(resenaDTO.ReservaId);
        if (reserva == null)
        {
            throw new InvalidOperationException("La reserva no existe.");
        }

        if (reserva.UsuarioInvitadoId != usuarioId)
        {
            throw new UnauthorizedAccessException("Solo el huésped de la reserva puede dejar una reseña.");
        }

        if (reserva.Estado != Reserva.EstadoEnum.Completada)
        {
            throw new InvalidOperationException("Solo puedes dejar una reseña si la reserva está Completada.");
        }

        bool yaTieneResena = await _resenaRepository.ExisteResenaPorReservaAsync(resenaDTO.ReservaId);
        if (yaTieneResena)
        {
            throw new InvalidOperationException("Ya has dejado una reseña para esta reserva. ¡Gracias por tu opinión!");
        }

        var nuevaResena = new Resena(resenaDTO.ReservaId, resenaDTO.Calificacion, resenaDTO.Comentario);
        await _resenaRepository.AgregarAsync(nuevaResena);

        return nuevaResena;
    }

    public async Task<IEnumerable<Resena>> ObtenerPorPropiedadAsync(int propiedadId)
    {
        return await _resenaRepository.ObtenerPorPropiedadIdAsync(propiedadId);
    }
}
