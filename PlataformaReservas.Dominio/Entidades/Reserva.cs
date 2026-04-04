namespace PlataformaReservas.Dominio;

public class Reserva
{
    public int Id { get; protected set; }

    public int PropiedadId { get; private set; }
    public int UsuarioInvitadoId { get; private set; }
    public DateTime FechaEntrada { get; private set; }
    public DateTime FechaSalida { get; private set; }

    public enum EstadoEnum
    {
        Confirmada, Cancelada, Completada
    }

    public EstadoEnum Estado { get; protected set; }



    public Reserva(int propiedadId,int usuarioInvitadoId, DateTime fechaEntrada, DateTime fechaSalida)
    {
        Estado = EstadoEnum.Confirmada;
        FechaEntrada= fechaEntrada;
        FechaSalida= fechaSalida;
        PropiedadId = propiedadId;
        UsuarioInvitadoId = usuarioInvitadoId;
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
        if (Estado != EstadoEnum.Confirmada || DateTime.UtcNow < FechaSalida)
        {
            throw new InvalidOperationException("No se puede completar la reserva. La reserva no está confirmada o la fecha de salida no ha pasado.");
        }

        Estado = EstadoEnum.Completada;
        
    }
    
}


