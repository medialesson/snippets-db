using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Karma
{
    [Route("snippets")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class VotingController
    {
        readonly IMediator _mediator;

        public VotingController(IMediator mediator)
        {
           _mediator = mediator; 
        }

        [HttpPost("{id}/upvote")]
        public async Task<VoteEnvelope> Upvote(string id)
        {
            var command = new Upvote.Command 
            {
                Vote = new Upvote.UserData 
                {
                    SnippetId = id
                } 
            };

            return await _mediator.Send(command);
        }

        [HttpPost("{id}/downvote")]
        public async Task<VoteEnvelope> Downvote(string id)
        {
            var command = new Downvote.Command 
            {
                Vote = new Downvote.UserData
                {
                    SnippetId = id
                }
            };

            return await _mediator.Send(command);
        }
    }
}