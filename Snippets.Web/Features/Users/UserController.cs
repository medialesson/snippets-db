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
    public class UserController
    {
        readonly IMediator _mediator;
        readonly ICurrentUserAccessor _currentUserAccessor;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
        {
            _mediator = mediator;
            _currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await _mediator.Send(new Details.Query
            {
                UserId = _currentUserAccessor.GetCurrentUserId()
            });
        }

        [HttpPut]
        public async Task<UserEnvelope> Update([FromBody] Edit.Command command) 
        {
            return await _mediator.Send(command);
        }
    }
}