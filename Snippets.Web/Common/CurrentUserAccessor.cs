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

        public string GetCurrentToken()
        {
            var headerValue = _httpContextAccessor.HttpContext.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value.ToString();
            if(headerValue.StartsWith("Bearer "))
            {
		        string token = headerValue.Substring("Bearer ".Length).Replace(" ", string.Empty);
		        if(token.Length > 0)
			        return token;
		    }
            return null;
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.User?.Claims
                ?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}