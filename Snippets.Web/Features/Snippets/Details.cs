using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using SQLitePCL;

namespace Snippets.Web.Features.Snippets
{
    public class Details
    {
        public class Query : IRequest<SnippetEnvelope>
        {
            public string ID { get; }

            public Query(string id)
            {
                ID = id;
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.ID).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, SnippetEnvelope>
        {
            private readonly SnippetsContext _context;

            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<SnippetEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var snippet = await _context.Snippets.GetAllData()
                    .FirstOrDefaultAsync(x => x.SnippetId == message.ID, cancellationToken);

                if (snippet != null)
                {
                    return new SnippetEnvelope(snippet);
                }
                else
                {
                    // TODO: Throw REST exception
                    throw new KeyNotFoundException("Snippet not found");
                }
            }
        }
    }
}
