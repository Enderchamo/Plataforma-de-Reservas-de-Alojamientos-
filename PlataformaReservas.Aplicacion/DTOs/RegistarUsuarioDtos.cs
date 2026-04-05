using System;
namespace PlataformaReservas.Aplicacion.DTOs;


public class RegistrarUsuarioDto
{

    public string Nombre {get; init;}

    public string Email {get; init;}

    public string Password{get; init;}

    public bool EsHost {get; init;}

    public bool EsGuest {get; init;}
}