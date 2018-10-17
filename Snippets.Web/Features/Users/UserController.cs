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
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await _mediator.Send(new Details.Query());
        }

        [HttpGet("{id}")]
        public async Task<UserEnvelope> GetUser(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpPut]
        public async Task<UserEnvelope> Update([FromBody] Edit.Command command) 
        {
            return await _mediator.Send(command);
        }
    }
}