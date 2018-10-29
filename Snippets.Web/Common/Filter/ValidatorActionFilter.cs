using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snippets.Web.Common.Exceptions;

namespace Snippets.Web.Common.Filter
{
    public class ValidatorActionFilter : IActionFilter
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a ValidationActionFilter 
        /// </summary>
        /// <param name="logger">Represents a type used to perform logging</param>
        public ValidatorActionFilter(ILogger<ValidatorActionFilter> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Called once the action gets executed, handles the action
        /// </summary>
        /// <param name="context">A context for action filters, specifically</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Compile a dictionary of the found errors
            // and hand it of to the ErrorHandlingMiddleware
            if (!context.ModelState.IsValid)
            {
                var result = new ContentResult();
                var messages = new Dictionary<string, string>();

                context.ModelState.ToList().ForEach(i => 
                    messages.Add(i.Key, i.Value.Errors.Select(y => y.ErrorMessage).FirstOrDefault()));

                // Error message that follows the apis exception convention (use for your own exceptions)
                throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, messages);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}