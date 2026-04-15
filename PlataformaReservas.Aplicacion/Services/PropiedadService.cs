using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;

namespace PlataformaReservas.Aplicacion.Services;

public class PropiedadService : IPropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IValidator<CrearPropiedadDto> _crearPropiedadValidator;

    public PropiedadService(IPropiedadRepository propiedadRepository, IValidator<CrearPropiedadDto> crearPropiedadValidator)
    {
        _propiedadRepository = propiedadRepository;
        _crearPropiedadValidator = crearPropiedadValidator;
    }

    public async Task ActualizarPropiedadAsync(int id, CrearPropiedadDto dto, int usuarioId)
    {
        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid)
        {
                throw new ValidationException(validacion.Errors);
        }

        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);
        if (propiedad == null)
        { 
                throw new InvalidOperationException("Propiedad no encontrada.");
        }

        //Usuario solo modifica propiedad que le pertenece.
        if (propiedad.HostId != usuarioId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para actualizar esta propiedad.");

        }

        propiedad.ActualizarDetalles(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad);

        await _propiedadRepository.ActualizarAsync(propiedad);

    }

    public async Task<IEnumerable<Propiedad>> BuscarPropiedadesAsync(string? ubicacion, decimal? prcioMaximo, int? capacidadMinimas, DateTime? fechaEntrada, DateTime? fechaSalida)
    {
        if (fechaEntrada.HasValue && fechaSalida.HasValue && fechaEntrada >= fechaSalida)
        {
            throw new ArgumentException("La fecha de entrada debe ser menor a la de salida.");
        }

        return await _propiedadRepository.BusquedaPorFiltroAsync(ubicacion, prcioMaximo, capacidadMinimas, fechaEntrada, fechaSalida);
    }

    public async Task<Propiedad> CrearPropiedadAsync(CrearPropiedadDto dto)
    {
        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        
        if (!validacion.IsValid)
        {
            throw new ValidationException(validacion.Errors);
        }


        //  Evitar propiedades duplicadas exactas del mismo Host.
        bool existeDuplicado = await _propiedadRepository.ExistePropiedadPorTituloYHostAsync(dto.Titulo, dto.HostId);
        if (existeDuplicado)
        {
            throw new InvalidOperationException($"Ya tienes una propiedad registrada con el título '{dto.Titulo}'. Por favor, verifica tus publicaciones.");
        }



        var nuevaPropiedad = new Propiedad(dto.Titulo, dto.Descripcion, dto.Ubicacion,dto.PrecioPorNoche,dto.Capacidad,dto.HostId);
        await _propiedadRepository.AgregarAsync(nuevaPropiedad);

        return nuevaPropiedad;

    }

    public async Task EliminarPropiedadAsync(int id, int usuarioId)
    {
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);
        
        if (propiedad == null)
        {
            throw new InvalidOperationException("Propiedad no encontrada.");
        }

        //Validacion para eliminar.

        if (propiedad.HostId != usuarioId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para eliminar esta propiedad");
        }

        await _propiedadRepository.EliminarAsync(propiedad);
    }

    public async Task<Propiedad?> ObtenerPorIdAsync(int id)
    {
        return await _propiedadRepository.ObtenerPorIdAsync(id);
    }
}
