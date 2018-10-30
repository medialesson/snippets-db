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
        readonly AppSettings _appSettings;
        readonly SendGridSender _sendGridClient;

        /// <summary>
        /// Initializes a SendGridMailService
        /// </summary>
        /// <param name="appSettings">Mapper for the "appsettings.json" file</param>
        public SendGridMailService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _sendGridClient = new SendGridSender(appSettings.SmtpConfig.Password);

            Email.DefaultSender = _sendGridClient;
            Email.DefaultRenderer = new RazorRenderer();
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, string from = null)
        {
            await Task.Run(() => 
            {
                throw new NotImplementedException();
            });
        }

        public async Task SendEmailFromTemplateAsync<T>(string to, string subject, string razorTemplatePath, T model)
        {
            await Email.From(_appSettings.SmtpConfig.Identity.Email, _appSettings.SmtpConfig.Identity.Name)
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(razorTemplatePath, model)
                .SendAsync();
        }
    }
}
