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

        /// <summary>
        /// Generates a valid Refresh token for a specific Jwt token
        /// </summary>
        /// <param name="jwtToken">Jwt token that belong to the user</param>
        /// <returns>Valid Refresh token for the specified Jwt token</returns>
        Task<string> CreateRefreshToken(string jwtToken);

        /// <summary>
        /// Validates a Refresh token with the Jwt token it has been generated from
        /// </summary>
        /// <param name="refreshToken">Refresh token that should be verified</param>
        /// <param name="jwtToken">Jwt token from which the Refresh token has been generated</param>
        /// <returns>Whether the Refresh token is valid</returns>
        Task<bool> VerifyRefreshToken(string refreshToken, string jwtToken);
    }
}