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
           /// <summary>
           /// Unique identifier of the Snippet the Vote will be submitted to
           /// </summary>
           public string SnippetId { get; set; }
       } 

        public class Command : IRequest<VoteEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Downvote VoteData
            /// </summary>
            public VoteData Vote { get; set; }
        } 

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Downvote CommandValidator
            /// </summary>
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

            /// <summary>
            /// Initializes a Downvote Handler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
               _context = context;
               _currentUserAccessor = currentUserAccessor;
               _mapper = mapper;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<VoteEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // Get the current user, snippet and karma from the database context
                var currentUserId = _currentUserAccessor.GetCurrentUserId();
                var selectedSnippet =  await _context.Snippets
                    .FindAsync(message.Vote.SnippetId, cancellationToken);
                var currentKarma = await _context.Karma.SingleOrDefaultAsync(x => 
                    x.Submitter.PersonId == currentUserId && x.Snippet == selectedSnippet, 
                    cancellationToken); 

                var vote = new Vote 
                {
                    Status = VoteStatus.Downvote
                };

                if (currentKarma == null)
                {
                    // Create a Karma that represents a Downvote
                    currentKarma = new Domains.Karma {
                        Upvote = false,
                        Snippet = selectedSnippet,
                        Submitter = await _context.Persons.FindAsync(currentUserId, cancellationToken)
                    };
                    await _context.Karma.AddAsync(currentKarma, cancellationToken);
                }
                else 
                {
                    if (currentKarma.Upvote)
                        // Update the Karma to represent a Downvote
                        currentKarma.Upvote = false;
                    else
                    {
                        // Delete the Karma that represents a Downvote
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