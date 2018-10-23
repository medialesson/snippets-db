using AutoMapper;
using MediatR;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Categories
{
    public class Details
    {
        public class Query : IRequest<CategoryEnvelope>
        {
            /// <summary>
            /// Initializes a Details Query
            /// </summary>
            /// <param name="categoryId">Unique identifier of the Category</param>
            public Query(string categoryId)
            {
                CategoryId = categoryId;
            }

            /// <summary>
            /// Unique identifier of the Category 
            /// </summary>
            public string CategoryId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CategoryEnvelope>
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
            public async Task<CategoryEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get the requested category from the database
                var selectedCategory = await _context.Categories
                    .FindAsync(new object[] { message.CategoryId }, cancellationToken: cancellationToken);

                if(selectedCategory != null)
                {
                    // Map from the data context to a transfer object
                    var category = _mapper.Map<Domains.Category, Category>(selectedCategory);
                    return new CategoryEnvelope(category);
                }
                else
                {
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"category.id", $"Category for id '{ message.CategoryId }' does not exist"}
                    });
                }
            }
        }
    }
}
