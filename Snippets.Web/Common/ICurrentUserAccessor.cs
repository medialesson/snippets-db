namespace Snippets.Web.Common
{
    public interface ICurrentUserAccessor
    {
        /// <summary>
        /// Returns the Jwt token for the currently authenticated user 
        /// </summary>
        /// <returns>Jwt token for the currently authenticated user</returns>
        string GetCurrentToken(); 
        
        /// <summary>
        /// Returns the currently authenticated user id
        /// </summary>
        /// <returns>User id of the currently authenticated user</returns>
        string GetCurrentUserId();
    }
}