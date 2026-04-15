using System;

namespace PlataformaReservas.Aplicacion.Interfaces;

public interface IEmailService
{   

    Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje);

}
