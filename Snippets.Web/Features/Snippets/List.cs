using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class List
    {
        public class Query : IRequest<SnippetsEnvelope>
        {
            public Query(string category, string authorId, int? limit, int? offset)
            {
                Category = category;
                AuthorId = authorId;
                Limit = limit;
                Offset = offset;
            }

            public string Category { get; }
            public string AuthorId { get; }
            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, SnippetsEnvelope>
        {
            private readonly SnippetsContext _context;

            public QueryHandler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<SnippetsEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                IQueryable<Snippet> queryable = _context.Snippets.GetAllData();

                if (!string.IsNullOrEmpty(message.Category))
                {
                    var category =
                        await _context.SnippetCategories.FirstOrDefaultAsync(x => x.CategoryId == message.Category,
                            cancellationToken);

                    if (category != null)
                    {
                        queryable = queryable.Where(x =>
                            x.SnippetCategories.Select(y => y.CategoryId).Contains(category.CategoryId));
                    }
                    else
                    {
                        return new SnippetsEnvelope();
                    }
                }

                if (!string.IsNullOrEmpty(message.AuthorId))
                {
                    var author =
                        await _context.Persons.FirstOrDefaultAsync(x => x.PersonId == message.AuthorId,
                            cancellationToken);

                    if (author != null)
                    {
                        queryable = queryable.Where(x => x.Author == author);
                    }
                    else
                    {
                        return new SnippetsEnvelope();
                    }
                }

                var snippets = await queryable
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new SnippetsEnvelope()
                {
                    Snippets = snippets,
                    SnippetsCount = snippets.Count()
                };
            }
        }
    }
}
