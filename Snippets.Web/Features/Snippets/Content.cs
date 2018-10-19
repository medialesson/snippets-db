using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Snippets
{
    public class Content
    {
        public class Query : IRequest<string>
        {
            /// <summary>
            /// Initializes a Content Query
            /// </summary>
            /// <param name="snippetId">Unique identifier of the Snippet the content gets retrieved from</param>
            public Query(string snippetId)
            {
                SnippetId = snippetId;
            }

            /// <summary>
            /// Unique identifier of the Snippet the content gets retrieved from
            /// </summary>
            public string SnippetId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, string>
        {
            private readonly SnippetsContext _context;

            /// <summary>
            /// Initializes a Content QueryHandler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }


            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<string> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get the requested Snippet from the database context
                var snippet = await _context.Snippets.GetAllData().FirstOrDefaultAsync(s => 
                    s.SnippetId == message.SnippetId, cancellationToken);

                if(snippet != null)
                {
                    return snippet.Content;
                }
                else
                {
                    throw new RestException(HttpStatusCode.NotFound, "Snippet not found");
                }
            }
        }
    }
}
