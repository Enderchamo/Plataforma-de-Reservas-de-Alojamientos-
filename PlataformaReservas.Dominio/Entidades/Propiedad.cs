using System.Data.Common;

namespace PlataformaReservas.Dominio;

public class Reserva
{
    public int Id { get; protected set; }
    public DateTime FechaEntrada { get; private set; }
    public DateTime FechaSalida { get; private set; }

    public enum EstadoEnum
    {
        Confirmada, Cancelada, Completada
    }

    public EstadoEnum Estado { get; protected set; }



    public Reserva(int id, DateTime fechaEntrada, DateTime fechaSalida)
    {
        this.Estado = EstadoEnum.Confirmada;
        this.FechaEntrada= fechaEntrada;
        this.FechaSalida= fechaSalida;
    }

    public void CancelarEstado()
    {
        if (Estado != EstadoEnum.Confirmada)
        {
            throw new InvalidOperationException("No se puede cancelar la reserva. La reserva no está confirmada.");
        }

        Estado = EstadoEnum.Cancelada;
    }

    public void CompletarEstado()
    {
        if (Estado != EstadoEnum.Confirmada || DateTime.Now < FechaSalida)
        {
            throw new InvalidOperationException("No se puede completar la reserva. La reserva no está confirmada o la fecha de salida no ha pasado.");
        }

        Estado = EstadoEnum.Completada;
        
    }
    
}


