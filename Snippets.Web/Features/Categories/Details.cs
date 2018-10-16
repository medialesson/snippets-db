using AutoMapper;
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
            private readonly IMapper _mapper;

            public QueryHandler(SnippetsContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CategoryEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var selectedCategory = await _context.Categories
                    .FindAsync(new object[] { message.ID }, cancellationToken: cancellationToken);

                if(selectedCategory != null)
                {
                    var category = _mapper.Map<Domains.Category, Category>(selectedCategory);

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
