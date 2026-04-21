using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Dominio.Entidades;
using PlataformaReservas.Dominio.Repositorios;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PlataformaReservas.Dominio.Excepciones;
using PlataformaReservas.Aplicacion.Helpers; 

namespace PlataformaReservas.Aplicacion.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;
    private readonly IValidator<LoginUsuarioDto> _loginValidator;
    private readonly IConfiguration _config;
    private readonly IUserContext _userContext;

    public UsuarioService(IUsuarioRepository usuarioRepository, IUserContext userContext, IEmailService emailService, IValidator<RegistrarUsuarioDto> registrarValidator, IValidator<LoginUsuarioDto> loginValidator, IConfiguration config)
    {
        _usuarioRepository = usuarioRepository;
        _userContext = userContext;
        _emailService = emailService;
        _registrarValidator = registrarValidator;
        _loginValidator = loginValidator;
        _config = config;
    }

    public async Task<Usuario> RegistrarUsuarioAsync(RegistrarUsuarioDto dto)
    {
        var validacion = await _registrarValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var existente = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        if (existente != null) throw new AppException("El correo ya está registrado.", 400, "ERROR_NEGOCIO");

        string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var nuevo = new Usuario(dto.Nombre, dto.Email, passwordHasheada, dto.EsHost, dto.EsGuest);
        
        nuevo.GenerarTokenConfirmacion(Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddHours(24));
        await _usuarioRepository.AgregarAsync(nuevo);

        string urlConfirmacion = $"http://localhost:5173/confirmar-cuenta?email={nuevo.Email}&token={nuevo.TokenConfirmacion}";
        
        // Generamos el HTML hermoso
        string htmlBody = PlantillasEmail.ObtenerPlantillaBase(
            titulo: $"¡Bienvenido a Air reservas, {nuevo.Nombre}!",
            mensaje: "Estamos felices de tenerte con nosotros. Para empezar a explorar alojamientos increíbles y gestionar tus reservas, confirma tu cuenta en el botón de abajo.",
            textoBoton: "Confirmar mi Cuenta",
            urlBoton: urlConfirmacion
        );

        // Enviamos el correo solo UNA vez
        await _emailService.EnviarCorreoAsync(nuevo.Email, "Bienvenido a Air reservas - Confirma tu cuenta", htmlBody);

        return nuevo;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUsuarioDto dto)
    {
        var validacion = await _loginValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password))
            throw new AppException("Credenciales incorrectas.", 400, "ERROR_NEGOCIO");

        if (!usuario.CuentaConfirmada)
            throw new AppException("Debes confirmar tu cuenta por correo antes de entrar.", 400, "ERROR_NEGOCIO");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _config["Jwt:Key"] ?? "NoTeMolestesEnIntentarAdivinarEstSUperClave1234Pollopica";
        var key = Encoding.UTF8.GetBytes(jwtKey);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        if (usuario.EsHost) claims.Add(new Claim(ClaimTypes.Role, "Host"));
        if (usuario.EsGuest) claims.Add(new Claim(ClaimTypes.Role, "Guest"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24), 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            Mensaje = "Login exitoso"
        };
    }

    public async Task ConfirmarCuentaAsync(string email, string token)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
        if (usuario == null) throw new AppException("Usuario no encontrado.", 400, "ERROR_NEGOCIO");

        usuario.ConfirmarCorreo(token);
        await _usuarioRepository.ActualizarAsync(usuario);
    }

    public async Task ActualizarRolesAsync(ActualizarRolDto dto)
    {
        var usuarioId = _userContext.UserId ?? throw new AppException("Sesión no válida.", 401, "NO_AUTORIZADO");
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
        
        if (usuario == null) 
            throw new AppException("Usuario no encontrado.", 404, "ERROR_NEGOCIO");

        usuario.ActualizarRoles(dto.EsHost, dto.EsGuest);
        await _usuarioRepository.ActualizarAsync(usuario);
    }
}