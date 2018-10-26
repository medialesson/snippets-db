using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Services
{
    public class SmtpMailService : IMailService
    {
        readonly AppSettings _appSettings;
        private SmtpSender _smtpClient;

        /// <summary>
        /// Initializes a SmtpMailService
        /// </summary>
        /// <param name="appSettings">Mapper for the "appsettings.json" file</param>
        public SmtpMailService(AppSettings appSettings)
        {
            _appSettings = appSettings;

            // Email client configuration
            Email.DefaultRenderer = new RazorRenderer();
            _smtpClient = new SmtpSender(new SmtpClient(_appSettings.SmtpConfig.Host, 25)
            {
                Credentials = new NetworkCredential(_appSettings.SmtpConfig.Username, _appSettings.SmtpConfig.Password)
            });
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, string from = null)
        {
            var email = Email.From(_appSettings.SmtpConfig.Identity.Email, _appSettings.SmtpConfig.Identity.Name)
                .To(to)
                .Subject(subject)
                .Body(htmlBody, true);

            var result = await _smtpClient.SendAsync(email);
        }

        public async Task SendEmailFromTemplateAsync<T>(string to, string subject, string razorTemplatePath, T model)
        {
            var email = Email.From(_appSettings.SmtpConfig.Identity.Email, _appSettings.SmtpConfig.Identity.Name)
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(razorTemplatePath, model);

            var result = await _smtpClient.SendAsync(email);
        }
    }
}
