using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Snippets.Web.Features.Categories
{
    [Route("categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of submitted categories
        /// </summary>
        /// <param name="limit">Limits the amount of items that are being returned (default: 25)</param>
        /// <param name="offset">Applies an offset to the categories collection</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CategoriesEnvelope> List([FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }
    }
}