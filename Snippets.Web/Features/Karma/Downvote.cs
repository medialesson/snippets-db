using System.Threading;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MediatR;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using System.Net;
using System.Threading.Tasks;
using Snippets.Web.Features.Karma.Enums;
using Microsoft.EntityFrameworkCore;

namespace Snippets.Web.Features.Karma
{
    public class Downvote
    {
       public class VoteData
       {
           public string SnippetId { get; set; }
       } 

        public class Command : IRequest<VoteEnvelope>
        {
            public VoteData Vote { get; set; }
        } 

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
               RuleFor(v => v.Vote).NotNull(); 
            }
        }

        public class Handler : IRequestHandler<Command, VoteEnvelope>
        {
            readonly SnippetsContext _context;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;

            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
               _context = context;
               _currentUserAccessor = currentUserAccessor;
               _mapper = mapper;
            }

            public async Task<VoteEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentUserId = _currentUserAccessor.GetCurrentUserId();
                var currentSnippet =  await _context.Snippets.FindAsync(message.Vote.SnippetId, cancellationToken);
                var currentKarma = await _context.Karma.SingleOrDefaultAsync(
                    x => x.Submitter.PersonId == currentUserId && x.Snippet == currentSnippet, 
                    cancellationToken); 

                var vote = new Vote 
                {
                    Status = VoteStatus.Downvote
                };

                if (currentKarma == null)
                {
                    currentKarma = new Domains.Karma {
                        Upvote = false,
                        Snippet = currentSnippet,
                        Submitter = await _context.Persons.FindAsync(currentUserId, cancellationToken)
                    };
                    await _context.Karma.AddAsync(currentKarma, cancellationToken);
                }
                else 
                {
                    if (currentKarma.Upvote)
                        currentKarma.Upvote = false;
                    else
                    {
                        _context.Karma.Remove(currentKarma);
                        vote.Status = VoteStatus.Removed;
                    };
                }

                await _context.SaveChangesAsync(cancellationToken);
                return new VoteEnvelope(vote);
            }
        }
    }
}