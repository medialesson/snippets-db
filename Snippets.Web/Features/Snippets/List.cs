using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;

namespace Snippets.Web.Features.Snippets
{
    public class List
    {
        public class Query : IRequest<SnippetsEnvelope>
        {
            /// <summary>
            /// Initializes a List Query
            /// </summary>
            /// <param name="category">Delimits the query to a specific category only</param>
            /// <param name="authorId">Delimits the query to a specific author only by its id</param>
            /// <param name="limit">Delimits the number of results returned</param>
            /// <param name="offset">Skips the specified amount of entries</param>
            public Query(string category, string authorId, int? limit, int? offset)
            {
                Category = category;
                AuthorId = authorId;
                Limit = limit;
                Offset = offset;
            }

            /// <summary>
            /// Delimits the query to a specific category only
            /// </summary>
            public string Category { get; }

            /// <summary>
            /// Delimits the query to a specific author only by its id
            /// </summary>
            public string AuthorId { get; }

            /// <summary>
            /// Delimits the number of results returned
            /// </summary>
            public int? Limit { get; }

            /// <summary>
            /// Skips the specified amount of entries
            /// </summary>
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, SnippetsEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IMapper _mapper;

            /// <summary>
            /// Initializes a List QueryHandler 
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
            public async Task<SnippetsEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get all Snippets form the database context 
                IQueryable<Domains.Snippet> queryable = _context.Snippets.GetAllData();

                if (!string.IsNullOrEmpty(message.Category))
                {
                    // Validate that the delimiting category exists
                    var category =
                        await _context.SnippetCategories.FirstOrDefaultAsync(x => 
                            x.CategoryId == message.Category,
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
                    // Validate that the delimiting author exists
                    var author =
                        await _context.Persons.FirstOrDefaultAsync(x => 
                            x.PersonId == message.AuthorId,
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

                var queriedSnippets = await queryable
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                // Map from the data context to a transfer object
                var snippets = _mapper.Map<List<Domains.Snippet>, List<Snippet>>(queriedSnippets);
                return new SnippetsEnvelope(snippets);
            }
        }
    }
}
