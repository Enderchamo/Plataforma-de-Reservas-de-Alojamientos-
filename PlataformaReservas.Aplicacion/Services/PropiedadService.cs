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
        // Forzamos el ID del Host desde el Token por seguridad
        dto.HostId = _userContext.UserId ?? throw new UnauthorizedAccessException("Sesión no válida.");

        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        if (await _propiedadRepository.ExistePropiedadPorTituloYHostAsync(dto.Titulo, dto.HostId))
        {
            throw new InvalidOperationException($"Ya tienes una propiedad con el título '{dto.Titulo}'.");
        }

        var nuevaPropiedad = new Propiedad(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad, dto.HostId);
        await _propiedadRepository.AgregarAsync(nuevaPropiedad);
        return nuevaPropiedad;
    }

    public async Task ActualizarPropiedadAsync(int id, CrearPropiedadDto dto)
    {
        var propiedad = await ValidarYObtenerPropiedadPropia(id);

        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        propiedad.ActualizarDetalles(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad);
        await _propiedadRepository.ActualizarAsync(propiedad);
    }

    public async Task ActualizarImagenAsync(int id, string rutaImagen)
{
    // 1. Obtenemos la propiedad y validamos permisos en un solo paso
    var propiedad = await ValidarYObtenerPropiedadPropia(id);

    // 2. CORRECCIÓN: El nombre de la variable es 'propiedad'
    propiedad.ImagenUrl = rutaImagen;

    // 3. Persistimos los cambios en la base de datos
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
            throw new ArgumentException("La fecha de entrada debe ser anterior a la de salida.");

        return await _propiedadRepository.BusquedaPorFiltroAsync(ubicacion, precioMaximo, capacidadMinimas, fechaEntrada, fechaSalida);
    }

    public async Task<Propiedad?> ObtenerPorIdAsync(int id) => await _propiedadRepository.ObtenerPorIdAsync(id);

    // --- Helper Privado de Seguridad ---
    private async Task<Propiedad> ValidarYObtenerPropiedadPropia(int id)
    {
        var usuarioId = _userContext.UserId ?? throw new UnauthorizedAccessException();
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);

        if (propiedad == null) 
            throw new KeyNotFoundException("La propiedad no existe.");

        if (propiedad.HostId != usuarioId)
            throw new UnauthorizedAccessException("No tienes permisos sobre esta propiedad.");

        return propiedad;
    }
}