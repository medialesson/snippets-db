using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Snippets.Web.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes an ErrorHandlingMiddleware 
        /// </summary>
        /// <param name="next">A function that can process a request</param>
        /// <param name="logger">Represents a type used to perform logging</param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        /// <summary>
        /// Processes the incoming requests
        /// </summary>
        /// <param name="context">Encapsulates all HTTP-specific information about a individual request</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                // Now we have something to do ...
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        /// <summary>
        /// Processes the individual exceptions and assigns their specific results
        /// </summary>
        /// <param name="context">Encapsulates all HTTP-specific information about an individual request</param>
        /// <param name="exception">Represents errors that occur during application execution</param>
        /// <param name="logger">Represents errors that occur during application execution</param>
        public static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandlingMiddleware> logger)
        {
            object errors = null;

            switch (exception)
            {
                case ValidationException ve:
                    throw new NotImplementedException();
                    break;
                // Generic errors that result in a json error response
                case RestException re:
                    errors = re.Errors;
                    context.Response.StatusCode = (int) re.Code;
                    context.Response.ContentType = "application/json";
                    logger.LogWarning(re, $"HTTP {(int) re.Code} - {re.Message}");
                    break;

                // Error that redirects the request to a different url
                case RedirectException re:
                    context.Response.Redirect(re.RedirectToUrl, re.IsPermanent);
                    break;

                // Errors of this kind result in http status code 500
                case Exception ex:
                    errors = string.IsNullOrWhiteSpace(ex.Message) ? "Error" : ex.Message;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    logger.LogError(ex, ex.Message);
                    break;
            }

            // Serialize the inbound error messages 
            var result = JsonConvert.SerializeObject(new
            {
                errors
            });

            await context.Response.WriteAsync(result);
        }
    }
}
