using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Snippets.Web.Common;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Users
{
    [Route("user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class UserController : Controller
    {
        readonly IMediator _mediator;

        /// <summary>
        /// Initializes a UserController
        /// </summary>
        /// <param name="mediator">Represents an instance of a MediateR mediator</param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves the details from the current user
        /// </summary>
        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await _mediator.Send(new Details.Query());
        }

        /// <summary>
        /// Retrieves a user by its unique identifier
        /// </summary>
        /// <param name="id">Unique identifier of the user</param>
        [HttpGet("{id}")]
        public async Task<UserEnvelope> GetUser(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

    	/// <summary>
        /// Edits the details of a user
        /// </summary>
        /// <param name="command">Command folowing the <see cref="Edit.UserData" /> convention</param>
        [HttpPut]
        public async Task<UserEnvelope> Edit([FromBody] Edit.Command command) 
        {
            return await _mediator.Send(command);
        }
    }
}