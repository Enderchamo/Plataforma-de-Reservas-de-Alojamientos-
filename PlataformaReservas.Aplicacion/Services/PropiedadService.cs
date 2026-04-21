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
    // 1. Declaramos el nuevo validador
    private readonly IValidator<ActualizarPropiedadDto> _actualizarPropiedadValidator; 
    private readonly IUserContext _userContext;

    private readonly IReservaRepository _reservaRepository;

    public PropiedadService(
        IPropiedadRepository propiedadRepository, 
        IValidator<CrearPropiedadDto> crearPropiedadValidator,
        IValidator<ActualizarPropiedadDto> actualizarPropiedadValidator, 
        IUserContext userContext,
        IReservaRepository reservaRepository)
    {
        _propiedadRepository = propiedadRepository;
        _crearPropiedadValidator = crearPropiedadValidator;
        // 3. Lo asignamos a la variable privada
        _actualizarPropiedadValidator = actualizarPropiedadValidator; 
        _userContext = userContext;
        _reservaRepository = reservaRepository;
    }

    public async Task<Propiedad> CrearPropiedadAsync(CrearPropiedadDto dto)
    {
        var hostId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        dto.HostId = hostId;

        var validacion = await _crearPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);
        
        if (await _propiedadRepository.ExistePropiedadPorTituloYHostAsync(dto.Titulo, dto.HostId))
        {
            throw new AppException($"Ya tienes una propiedad con el título '{dto.Titulo}'.", 400, "TITULO_DUPLICADO");
        }

        var nuevaPropiedad = new Propiedad(dto.Titulo, dto.Descripcion, dto.Ubicacion, dto.PrecioPorNoche, dto.Capacidad, dto.HostId);
        await _propiedadRepository.AgregarAsync(nuevaPropiedad);
        return nuevaPropiedad;
    }

    public async Task ActualizarPropiedadAsync(int id, ActualizarPropiedadDto dto)
    {
        // 4. Ejecutamos la validación antes de hacer cualquier consulta a la base de datos
        var validacion = await _actualizarPropiedadValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

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
        var tieneReservas = await _reservaRepository.TieneReservasLaPropiedadAsync(id);

        if (tieneReservas)
    {
    
        throw new AppException(
            "No es posible eliminar esta propiedad porque cuenta con registros de reservas en su historial.", 
            400, 
            "PROPIEDAD_CON_DEPENDENCIAS");
    }
        await _propiedadRepository.EliminarAsync(propiedad);
    }

    // --- Métodos de Lectura ---

    public async Task<IEnumerable<PropiedadListaDto>> BuscarPropiedadesAsync(string? ubicacion, decimal? precioMaximo, int? capacidadMinimas, DateTime? fechaEntrada, DateTime? fechaSalida)
    {
        if (fechaEntrada >= fechaSalida)
            throw new AppException("La fecha de entrada debe ser anterior a la de salida.", 400, "FECHAS_INVALIDAS");

        var propiedades = await _propiedadRepository.BusquedaPorFiltroAsync(ubicacion, precioMaximo, capacidadMinimas, fechaEntrada, fechaSalida);

        return propiedades.Select(p => new PropiedadListaDto
        {
            Id = p.Id,
            Titulo = p.Titulo,
            PrecioPorNoche = p.PrecioPorNoche,
            Ubicacion = p.Ubicacion,
            ImagenUrl = p.ImagenUrl,
            Capacidad = p.Capacidad,
            HostId = p.HostId
        });
    }

    public async Task<PropiedadDetalleDto?> ObtenerPorIdAsync(int id)
    {
        var propiedad = await _propiedadRepository.ObtenerPorIdAsync(id);
        
        if (propiedad == null) return null;

        return new PropiedadDetalleDto
        {
            Id = propiedad.Id,
            Titulo = propiedad.Titulo,
            Descripcion = propiedad.Descripcion,
            Ubicacion = propiedad.Ubicacion,
            PrecioPorNoche = propiedad.PrecioPorNoche,
            Capacidad = propiedad.Capacidad,
            ImagenUrl = propiedad.ImagenUrl,
            HostId = propiedad.HostId,
            NombreHost = propiedad.Host.Nombre
        };
    }

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