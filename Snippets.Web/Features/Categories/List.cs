using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Categories
{
    public class List
    {
        public class Query : IRequest<CategoriesEnvelope>
        {
            public Query(int? limit, int? offset)
            {
                Limit = limit;
                Offset = offset;
            }

            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CategoriesEnvelope>
        {
            private readonly SnippetsContext _context;

            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<CategoriesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                IQueryable<Category> queryable = _context.Categories;

                var categories = await queryable
                    .OrderByDescending(x => x.CategoryId)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new CategoriesEnvelope()
                {
                    Categories = categories,
                    CategoriesCount = categories.Count
                };
            }
        }
    }
}
