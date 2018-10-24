using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Services
{
    public class SendGridMailService : IMailService
    {
        readonly AppSettings _settings;
        readonly SendGridSender _sendGridClient;

        public SendGridMailService(AppSettings settings)
        {
            _settings = settings;
            _sendGridClient = new SendGridSender(settings.SmtpConfig.Password);

            Email.DefaultSender = _sendGridClient;
            Email.DefaultRenderer = new RazorRenderer();
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, string from = null)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailFromTemplateAsync<T>(string to, string subject, string razorTemplatePath, T model)
        {
            await Email.From(_settings.SmtpConfig.Identity.Email, _settings.SmtpConfig.Identity.Name)
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(razorTemplatePath, model)
                .SendAsync();
        }
    }
}
