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


namespace PlataformaReservas.Aplicacion.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;
    private readonly IValidator<LoginUsuarioDto> _loginValidator;
    private readonly IConfiguration _config;

    public UsuarioService(IUsuarioRepository usuarioRepository, IEmailService emailService, IValidator<RegistrarUsuarioDto> registrarValidator, IValidator<LoginUsuarioDto> loginValidator, IConfiguration config)
    {
        _usuarioRepository = usuarioRepository;
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
        if (existente != null) throw new InvalidOperationException("El correo ya está registrado.");

        string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var nuevo = new Usuario(dto.Nombre, dto.Email, passwordHasheada, dto.EsHost, dto.EsGuest);
        
        nuevo.GenerarTokenConfirmacion(Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddHours(24));
        await _usuarioRepository.AgregarAsync(nuevo);

        // Restauramos el envío de correo
        string urlConfirmacion = $"http://localhost:5173/confirmar-cuenta?email={nuevo.Email}&token={nuevo.TokenConfirmacion}";
        string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h1 style='color: #FF5A5F;'>¡Bienvenido, {nuevo.Nombre}!</h1>
            <a href='{urlConfirmacion}'>Verificar mi Cuenta</a>
        </div>";

        await _emailService.EnviarCorreoAsync(nuevo.Email, "Verifica tu cuenta", htmlBody);

        return nuevo;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUsuarioDto dto)
    {
        var validacion = await _loginValidator.ValidateAsync(dto);
        if (!validacion.IsValid) throw new ValidationException(validacion.Errors);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!usuario.CuentaConfirmada)
            throw new UnauthorizedAccessException("Debes confirmar tu cuenta por correo antes de entrar.");

        // Generamos el Token aquí (Logica de negocio limpia)
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
            Expires = DateTime.UtcNow.AddHours(1), 
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
        if (usuario == null) throw new InvalidOperationException("Usuario no encontrado.");

        usuario.ConfirmarCorreo(token);
        await _usuarioRepository.ActualizarAsync(usuario);
    }
}