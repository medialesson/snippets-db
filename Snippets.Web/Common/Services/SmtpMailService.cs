using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Services
{
    public class SmtpMailService : IMailService
    {
        readonly AppSettings _settings;

        public SmtpMailService(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(string to, string subject, string textBody, string htmlBody, string from = null)
        {
            using(SmtpClient client = new SmtpClient(_settings.SmtpConfig.Host, _settings.SmtpConfig.Port))
            {
                client.Credentials = new NetworkCredential(_settings.SmtpConfig.Username, _settings.SmtpConfig.Password);
                
                using(MailMessage mail = new MailMessage())
                {
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress(from ?? "no-reply@snippets-dev.azurewebsites.net", "Snippets DB");
                    mail.Subject = subject;
                    mail.Body = htmlBody ?? textBody;

                    mail.To.Insert(0, new MailAddress(to));
                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}
