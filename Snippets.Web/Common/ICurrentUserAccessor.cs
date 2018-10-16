namespace Snippets.Web.Common
{
    public interface ICurrentUserAccessor
    {
        /// <summary>
        /// Returns the currently authenticated user id
        /// </summary>
        /// <returns>User id of the currently authenticated user</returns>
        string GetCurrentUserId();
    }
}