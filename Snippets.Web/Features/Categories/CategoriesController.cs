using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Exceptions;

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
        public async Task<CategoriesEnvelope> List([FromQuery] int? limit = 25, [FromQuery] int? offset = 0)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }

        /// <summary>
        /// Retrieves a single category specified by its ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<CategoryEnvelope> Details(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }
    }
}