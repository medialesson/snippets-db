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

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("auth")]
        public async Task<UserEnvelope> Auth([FromBody] Auth.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}