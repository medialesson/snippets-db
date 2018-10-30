using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Services
{
    public interface IMailService
    {
        /// <summary>
        /// Sends out an email via a provider that is specified in the implementation
        /// </summary>
        /// <param name="to">Recipient of the sent email</param>
        /// <param name="subject">Subject of the sent email</param>
        /// <param name="htmlBody">Message that can contain html content</param>
        /// <param name="from">Sender of the email</param>
        Task SendEmailAsync(string to, string subject, string htmlBody, string from = null);

        /// <summary>
        /// Sends out an email from a template via a provider that is specified in the implementation
        /// </summary>
        /// <param name="to">Recipient of the sent email</param>
        /// <param name="subject">Subject of the sent email</param>
        /// <param name="razorTemplatePath">Path to the template used by the Razor template engine</param>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        Task SendEmailFromTemplateAsync<T>(string to, string subject, string razorTemplatePath, T model);
    }
}
