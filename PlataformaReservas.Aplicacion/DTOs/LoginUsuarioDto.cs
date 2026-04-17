using System;
namespace PlataformaReservas.Aplicacion.DTOs;


public class LoginUsuarioDto
{
    public string Email {get; init;}  = string.Empty;

    public string Password {get; init;}  = string.Empty;
}