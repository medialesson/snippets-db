using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Karma
{
    [Route("snippets")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class VotingController : Controller
    {
        readonly IMediator _mediator;

        /// <summary>
        /// Initializes a VotingController
        /// </summary>
        /// <param name="mediator">Represents an instance of a MediateR mediator</param>
        public VotingController(IMediator mediator)
        {
           _mediator = mediator; 
        }

        /// <summary>
        /// Submits or deletes an existing Upvote to the specified Snippet 
        /// </summary>
        /// <param name="id">Id of the Snippet to submit the upvote</param>
        [HttpPost("{id}/upvote")]
        public async Task<VoteEnvelope> Upvote(string id)
        {
            var command = new Upvote.Command 
            {
                Vote = new Upvote.VoteData 
                {
                    SnippetId = id
                } 
            };

            return await _mediator.Send(command);
        }
        /// <summary>
        /// Submits or deletes an existing Downvote to the specified Snippet 
        /// </summary>
        /// <param name="id">Id of the Snippet to submit the Downvote</param>
        [HttpPost("{id}/downvote")]
        public async Task<VoteEnvelope> Downvote(string id)
        {
            var command = new Downvote.Command 
            {
                Vote = new Downvote.VoteData
                {
                    SnippetId = id
                }
            };

            return await _mediator.Send(command);
        }
    }
}