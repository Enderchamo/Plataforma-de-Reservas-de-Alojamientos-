// PlataformaReservas.Dominio/Entidades/Reserva.cs
using PlataformaReservas.Dominio.Excepciones;

namespace PlataformaReservas.Dominio.Entidades;

public class Reserva
{
    public int Id { get; protected set; }

    public int PropiedadId { get; private set; }
    public int UsuarioInvitadoId { get; private set; }
    public DateTime FechaEntrada { get; private set; }
    public DateTime FechaSalida { get; private set; }

    public Propiedad Propiedad { get; private set; } = null!;
    public Usuario UsuarioInvitado { get; private set; } = null!;

    public enum EstadoEnum
    {
        Confirmada, Cancelada, Completada
    }

    public EstadoEnum Estado { get; protected set; }

    // Constructor para Entity Framework
    protected Reserva() { }

    public Reserva(int propiedadId, int usuarioInvitadoId, DateTime fechaEntrada, DateTime fechaSalida)
    {
        Estado = EstadoEnum.Confirmada;
        FechaEntrada = fechaEntrada;
        FechaSalida = fechaSalida;
        PropiedadId = propiedadId;
        UsuarioInvitadoId = usuarioInvitadoId;
    }

    public void CancelarEstado(DateTime fechaSolicitud)
    {
        if (Estado != EstadoEnum.Confirmada)
        {
            // 🛠️ Usamos AppException (400) para reglas de negocio
            throw new AppException("No se puede cancelar la reserva. La reserva no está confirmada.", 400);
        }

        if (fechaSolicitud.Date >= FechaEntrada.Date)
        {
            throw new AppException("Es demasiado tarde para cancelar esta reserva. Solo se permiten cancelaciones hasta un día antes de la llegada.", 400);
        }

        Estado = EstadoEnum.Cancelada;
    }

    public void CompletarEstado()
    {
        if (Estado != EstadoEnum.Confirmada)
        {
            throw new AppException($"No se puede completar esta reserva porque actualmente está en estado: {Estado}. Solo se pueden completar reservas Confirmadas.", 400);
        }
        
        // Validación de fecha para asegurar que la estancia terminó
        // if (DateTime.UtcNow < FechaSalida)
        // {
        //     throw new AppException($"Aún no puedes completar esta reserva. Podrás hacerlo después de la fecha de salida ({FechaSalida:dd/MM/yyyy HH:mm}).", 400);
        // }

        Estado = EstadoEnum.Completada;
    }
}