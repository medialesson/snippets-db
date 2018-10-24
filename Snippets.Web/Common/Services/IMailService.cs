using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody, string from = null);

        Task SendEmailFromEmbeddedAsync(string to, string subject, string razorTemplatePath, object model);
    }
}
