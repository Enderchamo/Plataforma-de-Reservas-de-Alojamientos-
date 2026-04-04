using System;
namespace PlataformaReservas.Aplicacion.DTOs;

public class CrearReservaDto
{
    public int PropiedadId {get; set;}

    public int UsuarioInvitadoId {get; set;}

    public DateTime FechaEntrada {get; set;}

    public DateTime FechaSalida {get; set;}

}
