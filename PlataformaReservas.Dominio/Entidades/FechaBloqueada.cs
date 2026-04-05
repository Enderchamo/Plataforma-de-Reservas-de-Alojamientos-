using System;

namespace PlataformaReservas.Dominio.Entidades;

public class FechaBloqueada
{
    public int Id { get; private set; }
    public int PropiedadId { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }

    public FechaBloqueada(int propiedadId, DateTime fechaInicio, DateTime fechaFin)
    {
   
        if (fechaFin < fechaInicio)
        {
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        PropiedadId = propiedadId;
        FechaInicio = fechaInicio.Date; 
        FechaFin = fechaFin.Date;
    }
}