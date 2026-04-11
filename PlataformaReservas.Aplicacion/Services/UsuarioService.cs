using System;
using System.Threading.Tasks;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;


namespace PlataformaReservas.Aplicacion.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;
    private readonly IValidator<LoginUsuarioDto> _loginValidator;

    public UsuarioService(IUsuarioRepository usuarioRepository,IValidator<RegistrarUsuarioDto> registrarValidator,IValidator<LoginUsuarioDto> loginValidator)
    {
        _usuarioRepository = usuarioRepository;
        _registrarValidator = registrarValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Usuario> RegistrarUsuarioAsync(RegistrarUsuarioDto dto)
    {
        var validacion = await _registrarValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var existente = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        if (existente != null) throw new InvalidOperationException("El correo ya está registrado.");

        var nuevo = new Usuario(dto.Nombre, dto.Email, dto.Password, dto.EsHost, dto.EsGuest);
        
        
        nuevo.GenerarTokenConfirmacion(Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddHours(24));

        await _usuarioRepository.AgregarAsync(nuevo);
        return nuevo;
    }

    public async Task<Usuario> LoginAsync(LoginUsuarioDto dto)
    {
        var validacion = await _loginValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        
        if (usuario == null || usuario.Password != dto.Password)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        // Un usuario no confirmado no puede iniciar sesión
        if (!usuario.CuentaConfirmada)
            throw new UnauthorizedAccessException("Debes confirmar tu cuenta por correo antes de entrar.");

        return usuario;
    }

    public async Task ConfirmarCuentaAsync(string email, string token)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
        if (usuario == null) throw new InvalidOperationException("Usuario no encontrado.");

        usuario.ConfirmarCorreo(token);
        await _usuarioRepository.ActualizarAsync(usuario);
    }
}
