using System;
namespace PlataformaReservas.Aplicacion.DTOs;

public class CrearReservaDto
{
    public int PropiedadId {get; init;}

    public int UsuarioInvitadoId {get; init;}

    public DateTime FechaEntrada {get; init;}

    public DateTime FechaSalida {get; init;}

}
