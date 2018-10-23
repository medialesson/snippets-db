using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common
{
    public class AppSettings
    { 
        /// <summary>
        /// Secret key for use with hash algorithms
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Accent colors for tag creation in string form (access with AccentColorsList)
        /// </summary>
        public string AccentColors { get; set; }

        /// <summary>
        /// Accent colors for tag creation in list form
        /// </summary>
        public IEnumerable<string> AccentColorsList { get => AccentColors.Split(';').ToList(); }

        /// <summary>
        /// SMTP config and client credentials
        /// </summary>
        public SmtpConfig SmtpConfig { get; set; }
    }

    public class SmtpConfig
    {
        /// <summary>
        /// User name for the SMTP client
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the SMTP client
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Port which is being used for the SMTP server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; }
    }
}
