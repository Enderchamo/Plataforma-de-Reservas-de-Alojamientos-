using System;
namespace PlataformaReservas.Aplicacion.DTOs;


public class RegistrarUsuarioDto
{

    public string Nombre {get; init;}  = string.Empty;

    public string Email {get; init;}  = string.Empty;

    public string Password{get; init;}  = string.Empty;

    public bool EsHost {get; init;} 

    public bool EsGuest {get; init;}
}