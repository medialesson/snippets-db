using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Snippets
{
    [Route("snippets")]
    [ApiController]
    public class SnippetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SnippetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        // [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SnippetEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}