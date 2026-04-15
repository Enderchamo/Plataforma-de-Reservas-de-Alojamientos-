using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PlataformaReservas.Aplicacion.Interfaces;

namespace PlataformaReservas.Infraestructura.Services;

public class EmailService : IEmailService
{
    private readonly string _emailOrigen = "gandergoku@gmail.com";
    private readonly string _contrasenaApp = "tyib evlk ikbh aocm";

    public Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
    {

        _ = Task.Run(async () =>
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
                
                Console.WriteLine($"Correo enviado a {destinatario}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }
        });

        return Task.CompletedTask;
    }
}
