namespace PlataformaReservas.Dominio.Entidades;

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

    public void CancelarEstado(DateTime fechaSolicitud)
    {
        if (Estado != EstadoEnum.Confirmada)
        {
            throw new InvalidOperationException("No se puede cancelar la reserva. La reserva no está confirmada.");
        }

        if (fechaSolicitud.Date >= FechaEntrada.Date)
        {
            throw new InvalidOperationException("Es demasiado tarde para cancelar esta reserva. Solo se permiten cancelaciones hasta un día antes de la llegada.");
        }

        Estado = EstadoEnum.Cancelada;
    }

    public void CompletarEstado()
    {
        if (Estado != EstadoEnum.Confirmada)
        {
            throw new InvalidOperationException($"No se puede completar esta reserva porque actualmente está en estado: {Estado}. Solo se pueden completar reservas Confirmadas.");
        }
        
        if (DateTime.UtcNow < FechaSalida)
        {
            throw new InvalidOperationException($"Aún no puedes completar esta reserva. Podrás hacerlo después de la fecha de salida ({FechaSalida:dd/MM/yyyy HH:mm}).");
        }

        Estado = EstadoEnum.Completada;
        Estado = EstadoEnum.Completada;
        
    }

    
}


