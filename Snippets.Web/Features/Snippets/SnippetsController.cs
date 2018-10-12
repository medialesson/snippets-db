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

        [HttpGet]
        public async Task<SnippetsEnvelope> Get([FromQuery] string category, [FromQuery] string author, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(category, author, limit, offset));
        }

        [HttpGet("{id}")]
        public async Task<SnippetEnvelope> Get(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpGet("{id}/raw")]
        public async Task<string> GetRawContent(string id)
        {
            return await _mediator.Send(new Content.Query(id));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SnippetEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}