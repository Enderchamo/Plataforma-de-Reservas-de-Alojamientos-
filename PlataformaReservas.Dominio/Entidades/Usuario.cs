using PlataformaReservas.Dominio.Entidades;

namespace PlataformaReservas.Dominio;

public class Usuario
{
    
    public int Id {get; private set;}
    public string Email {get; private set;}
    public string PasswordHash {get; private set;}
    public bool CuentaConfirmada {get; private set;}
    public bool EsHost{get; private set;}
    public bool EsGuest{get; private set;}

    public ICollection<Propiedad> Propiedads {get;private set;} = new List<Propiedad>();

    public Usuario(String email,String passWordHash, bool esHost, bool esGuest)
    {
        
        Email = email;
        PasswordHash = passWordHash;
        EsGuest = esGuest;
        EsHost = esHost;
        CuentaConfirmada = false;

    }

    public void ConfirmarCorreo()
    {
        CuentaConfirmada = true;
    }
    

}


