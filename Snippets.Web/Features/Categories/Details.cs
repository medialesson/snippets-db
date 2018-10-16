using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Categories
{
    public class Details
    {
        public class Query : IRequest<CategoryEnvelope>
        {
            public Query(string id)
            {
                ID = id;
            }

            public string ID { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CategoryEnvelope>
        {
            private readonly SnippetsContext _context;

            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<CategoryEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                Category category = await _context.Categories
                    .FindAsync(new object[] { message.ID }, cancellationToken: cancellationToken);

                if(category != null)
                {
                    return new CategoryEnvelope()
                    {
                        Category = category
                    };
                }
                else
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound, "Category not found");
                }
            }
        }
    }
}
