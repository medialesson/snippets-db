namespace Snippets.Web.Common.Security
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Generates a hash for password verfification 
        /// </summary>
        /// <param name="password">Password in plain text</param>
        /// <param name="salt">String that gets mixed in to secure the password hash</param>
        /// <returns></returns>
        byte[] Hash(string password, byte[] salt);
    }
}