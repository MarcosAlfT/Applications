using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Pagarte.Worker.Services
{
    public class EmailSenderService(
        IConfiguration configuration,
        ILogger<EmailSenderService> logger) : IEmailSenderService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<EmailSenderService> _logger = logger;

        public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var host = _configuration["Email:Host"]
                    ?? throw new InvalidOperationException("Email:Host not configured.");
                var port = _configuration.GetValue<int>("Email:Port");
                var username = _configuration["Email:Username"]
                    ?? throw new InvalidOperationException("Email:Username not configured.");
                var password = _configuration["Email:Password"]
                    ?? throw new InvalidOperationException("Email:Password not configured.");
                var fromEmail = _configuration["Email:From"]
                    ?? throw new InvalidOperationException("Email:From not configured.");
                var fromName = _configuration["Email:FromName"] ?? "Pagarte";

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);
                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                throw;
            }
        }
    }
}
