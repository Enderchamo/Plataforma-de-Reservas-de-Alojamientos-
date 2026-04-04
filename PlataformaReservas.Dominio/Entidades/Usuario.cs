using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio;

public class Usuario
{
    
    public int Id {get; private set;}

    public string Nombre { get; private set; }
    public string Email {get; private set;}
    public string PasswordHash {get; private set;}
    public bool CuentaConfirmada {get; private set;}
    public bool EsHost{get; private set;}
    public bool EsGuest{get; private set;}

    public string? TokenConfirmacion { get; private set; }
    public DateTime? FechaExpiracionToken { get; private set; }

    public ICollection<Propiedad> Propiedades {get;private set;} = new List<Propiedad>();
    public ICollection<Reserva> Reservas {get;private set;} = new List<Reserva>();
    public ICollection<Notificacion> Notificaciones {get;private set;} = new List<Notificacion>();


    public Usuario(string nombre,string email,string passwordHash, bool esHost, bool esGuest)
    {

        if (!esHost && !esGuest)
        {
            throw new ArgumentException("El usuario debe tener al menos un rol asignado (Host o Guest).");
        }


        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario no puede estar vacío.");
        }


        Nombre = nombre;
        Email = email;
        PasswordHash = passwordHash;
        EsGuest = esGuest;
        EsHost = esHost;
        CuentaConfirmada = false;

    }

    public void GenerarTokenConfirmacion(string token, DateTime fechaExpiracion)
    {
        TokenConfirmacion = token;
        FechaExpiracionToken = fechaExpiracion;
    }

    public void ConfirmarCorreo(string tokenIngresado)
    {
        if (TokenConfirmacion != tokenIngresado)
        {
            throw new InvalidOperationException("El token es inválido.");
        }
            
        if (DateTime.UtcNow > FechaExpiracionToken)
        {
            throw new InvalidOperationException("El token ha expirado.");
        }

        CuentaConfirmada = true;
        TokenConfirmacion = null;
        FechaExpiracionToken = null;
    }
    

}


