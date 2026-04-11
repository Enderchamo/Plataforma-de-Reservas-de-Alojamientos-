using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class FechaBloqueadaService : IFechaBloqueadaService
{

    private readonly IFechaBloqueadaRepository _fechaBloqueadaRepository;
    private readonly IPropiedadRepository _propiedadRepositoy;
    private readonly IValidator<BloquearFechaDto> _validator;

    public FechaBloqueadaService(IFechaBloqueadaRepository fechaBloqueadaRepository,IPropiedadRepository propiedadRepositoy, IValidator<BloquearFechaDto> validator)
    {
        _fechaBloqueadaRepository= fechaBloqueadaRepository;
        _propiedadRepositoy= propiedadRepositoy;
        _validator= validator;
    }
   
    public async Task<FechaBloqueada> BloquearFechaAsync(BloquearFechaDto dto, int hostId)
    {
        var validacion = await _validator.ValidateAsync(dto);
        if (!validacion.IsValid) 
        {
            throw new ValidationException(validacion.Errors);
        }

        var propiedad = await  _propiedadRepositoy.ObtenerPorIdAsync(dto.PropiedadId);
        if (propiedad == null)
        {
            throw new InvalidOperationException("La propiedad no existe.");
        }

        if (propiedad.HostId != hostId)

        {
            throw new UnauthorizedAccessException("Solo el dueño puede bloquear fechas en su propiedad.");
        }

        var nuevaFechaBloqueada = new FechaBloqueada(dto.PropiedadId, dto.FechaInicio, dto.FechaFin);
        await _fechaBloqueadaRepository.AgregarAsync(nuevaFechaBloqueada);

        return nuevaFechaBloqueada;
    }

    public async Task<IEnumerable<FechaBloqueada>> ObtenerPorPropiedadAsync(int propiedadId)
    {
        return await _fechaBloqueadaRepository.ObtenerPorPropiedadIdAsync(propiedadId);
    }
}
