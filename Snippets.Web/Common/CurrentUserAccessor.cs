using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Snippets.Web.Common
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

<<<<<<< HEAD
        public string GetCurrentUserId()
=======
        public string GetCurrentUsername()
>>>>>>> 1c87a7d... Add user accessor helper
        {
            return _httpContextAccessor.HttpContext.User?.Claims
                ?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}