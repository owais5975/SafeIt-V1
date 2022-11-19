
using Microsoft.Extensions.Options;
using SafeIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EBMS.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string recipient, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private MailSettings _settings;
        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendEmailAsync(string recipient, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(_settings.Mail);
                message.To.Add(recipient);
                message.Subject = subject;
                message.Body = body;

                SmtpClient SmtpServer = new SmtpClient(_settings.Host);
                SmtpServer.Port = _settings.Port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(_settings.Mail, _settings.Password);
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.EnableSsl = true;
                await SmtpServer.SendMailAsync(message);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
