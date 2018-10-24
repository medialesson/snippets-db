using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Snippets.Web.Common.Extensions;

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

        /// <summary>
        /// Initializes a RestException from a Dictionary
        /// </summary>
        /// <param name="code">HttpStatuscode returned by the server</param>
        /// <param name="dictionary">Dictionary containing a domain as key and the message as value</param>
        /// <returns>RestException folowing the error logging standard of the API</returns>
        public static RestException CreateFromDictionary(HttpStatusCode code, Dictionary<string, string> dictionary)
        {
            var errors = new JObject();

            foreach (var valuePair in dictionary)
            {
                var data = new JTokenWriter();
                data.WriteStartObject();

                var domain = valuePair.Key.ToLower().Split('.');
                var domainObject = domain.Last();

                foreach (var sub in domain)
                {   
                    data.WritePropertyName(sub);
                    if (sub != domainObject)
                        data.WriteStartObject();
                }

                data.WriteValue(valuePair.Value);

                foreach (var sub in domain)
                {
                    if (sub != domainObject)
                        data.WriteEndObject();
                }

                data.WriteEndObject();

                errors.Merge((JObject) data.Token, new JsonMergeSettings 
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            return new RestException(code, errors);
        }
    }
}
