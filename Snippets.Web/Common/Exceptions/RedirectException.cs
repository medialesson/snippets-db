using System;

namespace Snippets.Web.Common.Exceptions
{
    public class RedirectException : Exception
    {
        public RedirectException(string redirectToUrl, bool isPermanent)
        {
            RedirectToUrl = redirectToUrl;
            IsPermanent = isPermanent;
        }

        public string RedirectToUrl { get; }
        public bool IsPermanent { get; internal set; }
    }
}
