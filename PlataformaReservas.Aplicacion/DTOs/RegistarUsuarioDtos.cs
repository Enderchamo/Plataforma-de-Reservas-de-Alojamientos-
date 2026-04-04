using System;
namespace PlataformaReservas.Aplicacion.DTOs;


public class RegistrarUsuarioDto
{
    public string? Email {get; set;}

    public string? PasswordHash {get; set;}

    public bool EsHost {get; set;}

    public bool EsGuest {get; set;}
}