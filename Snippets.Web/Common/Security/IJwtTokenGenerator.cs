using System.Threading.Tasks;

namespace Snippets.Web.Common.Security
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a valid Jwt token for a specified user id
        /// </summary>
        /// <param name="userId">Id of the user the token is generated for</param>
        /// <returns>Valid Jwt token for specified user id</returns>
        Task<string> CreateToken(string userId);

        Task<string> CreateRefreshToken(string jwtToken, string passwordHash);
    }
}