using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Snippets.Web.Common.Exceptions;

namespace Snippets.Web.Common.Filter
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly List<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes a ValidationPipelineBehavior
        /// </summary>
        /// <param name="validators">List of validators for particular types</param>
        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators.ToList();
        }

        /// <summary>
        /// Processes the incoming requests 
        /// </summary>
        /// <param name="request">Incoming request that is handled</param>
        /// <param name="cancellationToken">Propagates the information that a task should be canceled</param>
        /// <param name="next">A function that can process a request</param>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();
            
            // Compile a dictionary of the found errors
            // and hand it of to the ErrorHandlingMiddleware
            if (failures.Count != 0)
            {
                var messages = new Dictionary<string, string>();

                failures.ForEach(i => messages.Add(i.PropertyName, i.ErrorMessage));

                // Error message that follows the apis exception convention (use for your own exceptions)
                throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, messages);
            }

            return await next();
        }
    }
}