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
            private readonly IMapper _mapper;

            public QueryHandler(SnippetsContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CategoriesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                IQueryable<Domains.Category> queryable = _context.Categories;

                var queriedCategories = await queryable
                    .OrderByDescending(x => x.CategoryId)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 25)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var categories = _mapper.Map<IList<Domains.Category>, IList<Category>>(queriedCategories);

                return new CategoriesEnvelope()
                {
                    Categories = categories,
                    CategoriesCount = categories.Count
                };
            }
        }
    }
}
