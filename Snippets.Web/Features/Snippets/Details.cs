using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;

namespace Snippets.Web.Features.Snippets
{
    public class Details
    {
        public class Query : IRequest<SnippetEnvelope>
        {
            /// <summary>
            /// Initializes a Details Query
            /// </summary>
            /// <param name="snippetId">Unique identifier of the Snippet from which the details are retrieved</param>
            public Query(string snippetId)
            {
                SnippetId = snippetId;
            }
            
            /// <summary>
            /// Unique identifier of the Snippet from which the details are retrieved           
            /// </summary>
            public string SnippetId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, SnippetEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IMapper _mapper;

            /// <summary>
            /// Initializes a Downvote Handler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            public QueryHandler(SnippetsContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<SnippetEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get the selected snippet from the database context
                var selectedSnippet = await _context.Snippets.GetAllData().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SnippetId == message.SnippetId, cancellationToken);

                if (selectedSnippet != null)
                {                    
                    // Map from the data context to a transfer object
                    var snippet = _mapper.Map<Domains.Snippet, Snippet>(selectedSnippet);
                    return new SnippetEnvelope(snippet);
                }
                else
                {
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"snippet.id", $"Snippet for id '{ message.SnippetId }' does not exist"}
                    });
                }
            }
        }
    }
}
