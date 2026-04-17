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

public class PropiedadService : IPropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IValidator<CrearPropiedadDto> _crearPropiedadValidator;
    private readonly IUserContext _userContext;

    public PropiedadService(
        IPropiedadRepository propiedadRepository, 
        IValidator<CrearPropiedadDto> crearPropiedadValidator,
        IUserContext userContext)
    {
        _propiedadRepository = propiedadRepository;
        _crearPropiedadValidator = crearPropiedadValidator;
        _userContext = userContext;
    }

    public async Task<Propiedad> CrearPropiedadAsync(CrearPropiedadDto dto)
    {
        // Extraemos el ID del Host desde el Token y lo asignamos
        var hostId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        dto.HostId = hostId;

        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);
        
        if (await _propiedadRepository.ExistePropiedadPorTituloYHostAsync(dto.Titulo, dto.HostId))
        {
            // Cambiado a AppException
            throw new AppException($"Ya tienes una propiedad con el título '{dto.Titulo}'.", 400, "TITULO_DUPLICADO");
        }

        var nuevaPropiedad = new Propiedad(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad, dto.HostId);
        await _propiedadRepository.AgregarAsync(nuevaPropiedad);
        return nuevaPropiedad;
    }

    public async Task ActualizarPropiedadAsync(int id, ActualizarPropiedadDto dto)
    {
        var hostId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);

        if (propiedad == null)
            throw new AppException("La propiedad solicitada no existe.", 404, "PROPIEDAD_NO_ENCONTRADA");

        if (propiedad.HostId != hostId)
            throw new AppException("No tienes permiso para modificar esta propiedad.", 403, "ACCESO_DENEGADO");

        propiedad.ActualizarDetalles(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad);
        await _propiedadRepository.ActualizarAsync(propiedad);
    }

    public async Task ActualizarImagenAsync(int id, string rutaImagen)
    {
        var propiedad = await ValidarYObtenerPropiedadPropia(id);
        propiedad.ImagenUrl = rutaImagen;
        await _propiedadRepository.ActualizarAsync(propiedad);
    }

    public async Task EliminarPropiedadAsync(int id)
    {
        var propiedad = await ValidarYObtenerPropiedadPropia(id);
        await _propiedadRepository.EliminarAsync(propiedad);
    }

    // --- Métodos de Lectura ---

    public async Task<IEnumerable<Propiedad>> BuscarPropiedadesAsync(string? ubicacion, decimal? precioMaximo, int? capacidadMinimas, DateTime? fechaEntrada, DateTime? fechaSalida)
    {
        if (fechaEntrada >= fechaSalida)
            throw new AppException("La fecha de entrada debe ser anterior a la de salida.", 400, "FECHAS_INVALIDAS");

        return await _propiedadRepository.BusquedaPorFiltroAsync(ubicacion, precioMaximo, capacidadMinimas, fechaEntrada, fechaSalida);
    }

    public async Task<Propiedad?> ObtenerPorIdAsync(int id) => await _propiedadRepository.ObtenerPorIdAsync(id);

    // --- Helper Privado de Seguridad ---
    private async Task<Propiedad> ValidarYObtenerPropiedadPropia(int id)
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);

        if (propiedad == null) 
            throw new AppException("Propiedad no encontrada.", 404, "NO_ENCONTRADA");

        if (propiedad.HostId != usuarioId)
            throw new AppException("No tienes permisos sobre esta propiedad.", 403, "ACCESO_DENEGADO");

        return propiedad;
    }
}