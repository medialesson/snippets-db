using System;
using System.Net;

namespace Snippets.Web.Common.Exceptions
{
    public class RestException : Exception
    {
        /// <summary>
        /// Initializes a RestException
        /// </summary>
        /// <param name="code">HttpStatuscode returned by the server</param>
        /// <param name="errors">Json object returned alongside the HttpStatuscode</param>
        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }

        /// <summary>
        /// HttpStatuscode returned by the server
        /// </summary>
        public HttpStatusCode Code { get; internal set; }

        /// <summary>
        /// Json object returned alongside the HttpStatuscode
        /// </summary>
        public object Errors { get; internal set; }
    }
}
