using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Snippets.Web.Features.Categories
{
    [Route("categories")]
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a CategoriesController
        /// </summary>
        /// <param name="mediator">Represents an instance of a MediatR mediator</param>
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of submitted categories
        /// </summary>
        /// <param name="limit">Limits the amount of items that are being returned (default: 25)</param>
        /// <param name="offset">Applies an offset to the categories collection</param>
        [HttpGet]
        public async Task<CategoriesEnvelope> List([FromQuery] int? limit = 25, [FromQuery] int? offset = 0)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }

        /// <summary>
        /// Retrieves a single category specified by its unique identifier
        /// </summary>
        /// <param name="id">Unique identifier of the Category</param>
        [HttpGet("{id}")]
        public async Task<CategoryEnvelope> Details(string id)
        {
            return await _mediator.Send(new Details.Query(id));
        }
    }
}