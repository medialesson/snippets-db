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
            public Query(string snippetId)
            {
                SnippetId = snippetId;
            }

            public string SnippetId { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.SnippetId).NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, string>
        {
            private readonly SnippetsContext _context;

            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(Query message, CancellationToken cancellationToken)
            {
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
