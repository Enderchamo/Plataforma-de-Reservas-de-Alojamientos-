using System;

namespace PlataformaReservas.Aplicacion.DTOs;

public class BloquearFechaDto
{
    public int PropiedadId { get; init; } 
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }

}