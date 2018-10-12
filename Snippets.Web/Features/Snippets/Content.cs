using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Snippets
{
    public class Content
    {
        public class Query : IRequest<string>
        {
            public Query(string id)
            {
                ID = id;
            }

            public string ID { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.ID).NotNull().NotEmpty();
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
                    s.SnippetId == message.ID, cancellationToken);

                if(snippet != null)
                {
                    return snippet.Content;
                }
                else
                {
                    // TODO: Throw REST exception
                    throw new Exception("Snippet not found");
                }
            }
        }
    }
}
