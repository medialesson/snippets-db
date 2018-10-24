using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Snippets.Web.Features.Users
{
    [Route("users")]
    public class UsersController : Controller
    {
        readonly IMediator _mediator;

        /// <summary>
        /// Initializes a UsersController
        /// </summary>
        /// <param name="mediator">Represents an instance of a MediateR mediator</param>
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="command">Command folowing the <see cref="Create.UserData" /> convention</param>
        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// Authenticates an existing user
        /// </summary>
        /// <param name="command">Command folowing the <see cref="Auth.UserData" /> convention</param>
        [HttpPost("auth")]
        public async Task<UserEnvelope> Auth([FromBody] Auth.Command command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// Generates a new Jwt token from a Refresh token
        /// </summary>
        /// <param name="refresh">Valid Refresh token for the current Jwt token</param>
        [HttpPost("auth/refresh")]
        public async Task<UserTokensEnvelope> Refresh([FromHeader] string refresh)
        {
            return await _mediator.Send(new Refresh.Command
             {
                Tokens = new Refresh.UserTokensData 
                {
                    Refresh = refresh
                }
            });
        }

        /// <summary>
        /// Revokes an existing Refresh token
        /// </summary>
        /// <param name="refresh">Valid Refresh token for the current Jwt token</param>
        [HttpPost("auth/revoke")]
        public async Task<object> Revoke([FromHeader] string refresh)
        {
            return await _mediator.Send(new Revoke.Command
             {
                Tokens = new Revoke.UserTokensData 
                {
                    Refresh = refresh
                }
            });
        }

        /// <summary>
        /// Verifies the account by its verification key
        /// </summary>
        /// <param name="command">Command following the <see cref="Verify.VerificationData"/> model</param>
        /// <returns></returns>
        [HttpPost("auth/verify")]
        public async Task<IActionResult> Verify([FromBody] Verify.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}