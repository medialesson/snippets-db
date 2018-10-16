using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Users
{
    [Route("user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly ICurrentUserAccessor _currentUserAccessor;
        readonly SnippetsContext _context;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor, SnippetsContext context)
        {
            _mediator = mediator;
            _currentUserAccessor = currentUserAccessor;
            _context = context;
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

        [HttpPost]
        public async Task<IActionResult> Test()
        {
            string id = _currentUserAccessor.GetCurrentUserId();
            var p = await _context.Persons.FindAsync(id);

            p.Preferences.IsProfilePublic = true;

            _context.Persons.Update(p);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}