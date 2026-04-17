using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PlataformaReservas.Aplicacion.Interfaces;
using System;
using System.Threading.Tasks;

namespace PlataformaReservas.Infraestructura.Services;

public class EmailService : IEmailService
{
    private readonly string _emailOrigen = "gandergoku@gmail.com";
    private readonly string _contrasenaApp = "tyib evlk ikbh aocm";

    public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Plataforma de Reservas", _emailOrigen));
            email.To.Add(new MailboxAddress("", destinatario));
            email.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = mensaje };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailOrigen, _contrasenaApp);
            
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            
            Console.WriteLine($"Correo enviado a {destinatario} - Asunto: {asunto}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR AL ENVIAR CORREO a {destinatario}: {ex.Message}");
            // AQUÍ ESTÁ LA MAGIA: Eliminamos el 'throw;' para no abortar la transacción de la base de datos
        }
    }
}