using System.Threading.Tasks;

namespace Snippets.Web.Common.Security
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a valid Jwt token for a specefied user id
        /// </summary>
        /// <param name="userId">Id of the user the token is generated for</param>
        /// <returns>Valid Jwt token for specfied user id</returns>
        Task<string> CreateToken(string userId);
    }
}