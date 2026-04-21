using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using PlataformaReservas.Aplicacion.Interfaces;
using System.Threading.Tasks;
using System;

namespace PlataformaReservas.Infraestructura.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensajeHtml)
        {
            var smtpUser = _config["Smtp:User"]; 
            var smtpPass = _config["Smtp:Pass"];
            var smtpHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = _config["Smtp:Port"] ?? "587";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                throw new InvalidOperationException("Faltan las credenciales del correo en appsettings.json. Verifica las llaves 'Smtp:User' y 'Smtp:Pass'.");
            }

            var email = new MimeMessage();
            
            // Nombre profesional del remitente
            email.From.Add(new MailboxAddress("Air reservas", smtpUser));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = asunto;

            var builder = new BodyBuilder();
            
            // Cuerpo del mensaje (Plan A y Plan B)
            builder.HtmlBody = mensajeHtml;
            builder.TextBody = "Para ver tu confirmación de Air reservas, habilita la vista HTML en tu correo. Enlace: " + mensajeHtml;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(smtpHost, int.Parse(smtpPort), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUser, smtpPass);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}