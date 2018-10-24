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
        readonly AppSettings _appSettings;

        public SmtpMailService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task SendEmailAsync(string to, string subject, string textBody, string htmlBody, string from = null)
        {
            using(SmtpClient client = new SmtpClient(_appSettings.SmtpConfig.Host, _appSettings.SmtpConfig.Port))
            {
                client.Credentials = new NetworkCredential(_appSettings.SmtpConfig.Username, _appSettings.SmtpConfig.Password);
                
                using(MailMessage mail = new MailMessage())
                {
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress(from ?? _appSettings.SmtpConfig.Identity.Email, _appSettings.SmtpConfig.Identity.Name);
                    mail.Subject = subject;
                    mail.Body = htmlBody ?? textBody;

                    mail.To.Insert(0, new MailAddress(to));
                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}
