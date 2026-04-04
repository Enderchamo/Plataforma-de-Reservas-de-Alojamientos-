using System;
namespace PlataformaReservas.Aplicacion.DTOs;

public class CrearPropiedadDto
{
    public string? Titulo {get;  set;}

    public string? Direccion {get;set;}

    public decimal PrecioPorNoche {get; set;}

    public int HostId {get; set;}


}
