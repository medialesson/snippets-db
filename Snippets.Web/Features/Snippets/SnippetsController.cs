using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Security;
using Snippets.Web.Features.Languages;

namespace Snippets.Web.Features.Snippets
{
    [Route("snippets")]
    public class SnippetsController : Controller
    {
        private readonly IMediator _mediator;
 
        /// <summary>
        /// Initializes a SnippetsController
        /// </summary>
        /// <param name="mediator">Represents an instance of a MediateR mediator</param>
        public SnippetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of Snippets
        /// </summary>
        /// <param name="category">Delimits the query to a specific category only</param>
        /// <param name="author">Delimits the query to a specific author only by its id</param>
        /// <param name="limit">Delimits the number of results returned</param>
        /// <param name="offset">Skips the specified amount of entries</param>
        [HttpGet]
        public async Task<SnippetsEnvelope> Get([FromQuery] string category, [FromQuery] string author, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(category, author, limit, offset));
        }

        /// <summary>
        /// Retrieves a single Snippet by its unique identifier
        /// </summary>
        /// <param name="id">Unique identifier of the Snippet</param>
        [HttpGet("{id}")]
        public async Task<SnippetEnvelope> Get(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        /// <summary>
        /// Retrieves the content of a Snippet by its unique identifier
        /// </summary>
        /// <param name="id">Unique identifier of the Snippet</param>
        [HttpGet("{id}/raw")]
        public async Task<string> GetRawContent(string id)
        {
            return await _mediator.Send(new Content.Query(id));
        }

        /// <summary>
        /// Deletes a single Snippet by its unique identifier
        /// </summary>
        /// <param name="id">Unique identifier of the Snippet</param>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<object> Delete(string id)
        {
            return await _mediator.Send(new Delete.Command
            {
                Snippet = new Delete.SnippetData
                {
                    SnippetId = id
                }
            });
        }

        /// <summary>
        /// Edits the details of a Snippet by its unique identifier 
        /// </summary>
        /// <param name="id">Unique identifier of the Snippet</param>
        /// <param name="command">Command folowing the <see cref="Edit.SnippetData" /> convention</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SnippetEnvelope> Edit(string id, [FromBody] Edit.Command command)
        {
            command.SnippetId = id;
            return await _mediator.Send(command);
        }

    	/// <summary>
        /// Creates a new Snippet
        /// </summary>
        /// <param name="command">Command folowing the <see cref="Create.SnippetData" /> convention</param>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SnippetEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// Retrieves a list of all registered Language enums
        /// </summary>
        [HttpGet("languages")]
        public async Task<LanguagesEnvelope> ListLanguages()
        {
            return await _mediator.Send(new Languages.List.Query());
        }
    }
}