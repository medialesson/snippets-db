using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;

namespace Snippets.Web.Features.Snippets
{
    public class Details
    {
        public class Query : IRequest<SnippetEnvelope>
        {
            public Query(string snippetId)
            {
                SnippetId = snippetId;
            }
            
            public string SnippetId { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.SnippetId).NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, SnippetEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IMapper _mapper;

            public QueryHandler(SnippetsContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<SnippetEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var selectedSnippet = await _context.Snippets.GetAllData().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SnippetId == message.SnippetId, cancellationToken);

                if (selectedSnippet != null)
                {                    
                    var snippet = _mapper.Map<Domains.Snippet, Snippet>(selectedSnippet);
                    return new SnippetEnvelope(snippet);
                }
                else
                {
                    throw new RestException(HttpStatusCode.NotFound, "Snippet not found");
                }
            }
        }
    }
}
