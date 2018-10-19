using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Snippets.Web.Common
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a CurrentUserAccessor
        /// </summary>
        /// <param name="httpContextAccessor">Accessor for the HttpContext</param>
        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.User?.Claims
                ?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        // TODO: Method GetCurrentUserToken()
    }
}