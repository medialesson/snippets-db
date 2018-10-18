using AutoMapper;
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
            /// <summary>
            /// Initializes a List Query
            /// </summary>
            /// <param name="limit">Delimits the number of results returned</param>
            /// <param name="offset">Skips the specified amount of entries</param>
            public Query(int? limit, int? offset)
            {
                Limit = limit;
                Offset = offset;
            }

            /// <summary>
            /// Delimits the number of results returned
            /// </summary>
            public int? Limit { get; }

            /// <summary>
            /// Skips the specified amount of entries
            /// </summary>
            /// <value></value>
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CategoriesEnvelope>
        {
            private readonly SnippetsContext _context;
            private readonly IMapper _mapper;

            /// <summary>
            /// Initializes a Details QueryHandler 
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
            public async Task<CategoriesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get all categories from the database context
                IQueryable<Domains.Category> queryable = _context.Categories;

                var queriedCategories = await queryable
                    .OrderByDescending(x => x.CategoryId)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                // Map from the data context to a transfer object
                var categories = _mapper.Map<IList<Domains.Category>, IList<Category>>(queriedCategories);
                return new CategoriesEnvelope(categories);
            }
        }
    }
}
