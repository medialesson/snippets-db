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
    public class Upvote
    {
       public class VoteData
       {
           /// <summary>
           /// Unique identifier of the Snippet the Vote will be submitted to
           /// </summary>
           public string SnippetId { get; set; }
       } 

        public class VoteDataValidator : AbstractValidator<VoteData>
        {
            /// <summary>
            /// Initializes a Upvote VoteDataValidator
            /// </summary>
            public VoteDataValidator()
            {
                RuleFor(x => x.SnippetId).NotNull().WithMessage("Id has to have a value");
            } 
        } 

        public class Command : IRequest<VoteEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Upvote VoteData
            /// </summary>
            public VoteData Vote { get; set; }
        } 

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Upvote CommandValidator
            /// </summary>
            public CommandValidator()
            {
               RuleFor(v => v.Vote).NotNull().WithMessage("Payload has to contain a vote object"); 
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

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<VoteEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // Get the current user, snippet and karma from the database context
                var currentUserId = _currentUserAccessor.GetCurrentUserId();
                var currentSnippet =  await _context.Snippets
                    .FindAsync(message.Vote.SnippetId, cancellationToken);
                var currentKarma = await _context.Karma.SingleOrDefaultAsync(x => 
                    x.Submitter.PersonId == currentUserId && x.Snippet == currentSnippet, 
                    cancellationToken); 

                var vote = new Vote 
                {
                    Status = VoteStatus.Upvote
                };

                if (currentKarma == null)
                {
                    // Create a Karma that represents a Upvote
                    currentKarma = new Domains.Karma {
                        Upvote = true,
                        Snippet = currentSnippet,
                        Submitter = await _context.Persons.FindAsync(currentUserId, cancellationToken)
                    };
                    await _context.Karma.AddAsync(currentKarma, cancellationToken);
                }
                else 
                {
                    // Update the Karma to represent a Upvote
                    if (!currentKarma.Upvote)
                        currentKarma.Upvote = true;
                    else
                    {
                        // Delete the Karma that represents a Upvote
                        _context.Karma.Remove(currentKarma);
                        vote.Status = VoteStatus.Removed;
                    }
                } 

                await _context.SaveChangesAsync(cancellationToken);
                return new VoteEnvelope(vote);
            }
        }
    }
}