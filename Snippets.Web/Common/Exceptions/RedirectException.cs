using System;

namespace Snippets.Web.Common.Exceptions
{
    public class RedirectException : Exception
    {
        /// <summary>
        /// Initializes a RedirectException 
        /// </summary>
        /// <param name="redirectToUrl">Url to which the requesting instance is redirected</param>
        /// <param name="isPermanent">Whether the redirection is permanent</param>
        public RedirectException(string redirectToUrl, bool isPermanent)
        {
            RedirectToUrl = redirectToUrl;
            IsPermanent = isPermanent;
        }

        /// <summary>
        /// Url to which the requesting instance is redirected
        /// </summary>
        public string RedirectToUrl { get; }

        /// <summary>
        /// Whether the redirection is permanent
        /// </summary>
        public bool IsPermanent { get; internal set; }
    }
}
