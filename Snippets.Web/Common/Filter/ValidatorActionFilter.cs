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

        public ValidatorActionFilter(ILogger<ValidatorActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ContentResult();
                var messages = new Dictionary<string, string>();

                context.ModelState.ToList().ForEach(i => 
                    messages.Add(i.Key, i.Value.Errors.Select(y => y.ErrorMessage).FirstOrDefault()));

                throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, messages);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}