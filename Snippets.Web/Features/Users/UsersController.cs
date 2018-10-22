using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Snippets.Web.Features.Users
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
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
        /// <param name="refreshToken">Valid Refresh token for the current Jwt token</param>
        /// <returns></returns>
        [HttpPost("auth/refresh")]
        public async Task<UserTokensEnvelope> Refresh([FromHeader] string refreshToken)
        {
            return await _mediator.Send(new Refresh.Command
             {
                Tokens = new Refresh.UserTokensData 
                {
                    RefreshToken = refreshToken
                }
            });
        }
    }
}