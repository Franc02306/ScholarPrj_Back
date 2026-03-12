using ScholarPrj_Back.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace ScholarPrj_Back.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly EmailTemplates _emailTemplates;
        private readonly ScholarPrjSettings _scholarPrjSettings;

        public EmailService(IOptions<EmailSettings> emailSettings, IOptions<EmailTemplates> emailTemplates, IOptions<ScholarPrjSettings> scholarPrjSettings)
        {
            _emailSettings = emailSettings.Value;
            _emailTemplates = emailTemplates.Value;
            _scholarPrjSettings = scholarPrjSettings.Value;
        }

        /// <summary>
        /// Envia un correo al nuevo usuario con las credenciales generadas por el sistema
        /// </summary>
        public async Task SendUserCredentialAsync(string email, string username, string password, bool isFemale)
        {
            var basePath = AppContext.BaseDirectory;
            var templatePath = Path.Combine(basePath, _emailTemplates.WelcomeTemplate);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"No se encontró la plantilla: {templatePath}");

            var html = await File.ReadAllTextAsync(templatePath);
            var greeting = isFemale ? "Bienvenida" : "Bienvenido";

            // Determinar entorno
            var baseUrl = _scholarPrjSettings.WebProd
                ? _scholarPrjSettings.LinkProd
                : _scholarPrjSettings.LinkDev;

            html = html
                .Replace("{{GREETING}}", greeting)
                .Replace("{{USERNAME}}", username)
                .Replace("{{PASSWORD}}", password)
                .Replace("{{WEB_LINK}}", baseUrl);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Credenciales de acceso - Web Scholar";

            message.Body = new TextPart("html")
            {
                Text = html
            };

            await SendAsync(message);
        }

        /// <summary>
        /// Método base para enviar correos (conecta, envía y desconecta)
        /// </summary>
        private async Task SendAsync(MimeMessage message)
        {
            using var client = new SmtpClient();

            var secureOption = _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, secureOption);

            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
