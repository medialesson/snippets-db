using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Common.Services;

namespace Snippets.Web.Features.Snippets
{
    public class Delete
    {
        public class SnippetData
        {
            /// <summary>
            /// Unique identifier of the Snippet from which the details are retrieved           
            /// </summary>
            [JsonProperty("id")]
            public string SnippetId { get; set; } 
        }

        public class SnippetDataValidator : AbstractValidator<SnippetData>
        {
            public SnippetDataValidator()
            {
                RuleFor(x => x.SnippetId).NotEmpty().WithMessage("Id has to have a value"); 
            }
        }

        public class Command : IRequest<object>
        {
            public SnippetData Snippet { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Create CommandValidator
            /// </summary>
            public CommandValidator()
            {
                RuleFor(x => x.Snippet)
                    .NotNull().WithMessage("Payload has to contain a snippet object")
                    .SetValidator(new SnippetDataValidator());
            }
        }
        
        public class Handler : IRequestHandler<Command, object>
        {
            readonly SnippetsContext _context;
            readonly ICurrentUserAccessor _currentUserAccessor;

            /// <summary>
            /// Initializes a Create Handler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<object> Handle(Command message, CancellationToken cancellationToken)
            {
                var author = await _context.Persons.FirstAsync(p => 
                    p.PersonId == _currentUserAccessor.GetCurrentUserId(), 
                    cancellationToken);
                var selectedSnippet = await _context.Snippets.SingleOrDefaultAsync(x => x.SnippetId == message.Snippet.SnippetId);

                if (selectedSnippet != null)
                {     
                    if (selectedSnippet.Author != author)
                        throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                        {
                            {"snippet", $"User for id '{ author.PersonId }' is not the author of Snippet for id '{ message.Snippet.SnippetId }'"}
                        });

                    _context.Snippets.Remove(selectedSnippet);

                    await _context.SaveChangesAsync(cancellationToken);
                    throw RestException.CreateFromDictionary(HttpStatusCode.OK, new Dictionary<string, string>
                    {
                        {"snippet", $"Snippet for id '{ selectedSnippet.SnippetId }' has been deleted"}
                    });
                }
                else
                {
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"snippet.id", $"Snippet for id '{ message.Snippet.SnippetId }' does not exist"}
                    });
                }
            }
        }
    }
}