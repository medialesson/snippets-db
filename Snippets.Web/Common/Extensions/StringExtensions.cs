namespace Snippets.Web.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a url save string from a base64 encoded string
        /// </summary>
        /// <param name="str">String that is base64 encoded</param>
        /// <returns>Base64 string that is url save</returns>
        public static string ToURLSave(this string str)
        {
            return str.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        } 

        /// <summary>
        /// Returns a non url save base64 string
        /// </summary>
        /// <param name="str">Base64 string that is url save</param>
        /// <returns>String that is base64 encoded</returns>
        public static string FromURLSave(this string str)
        {
            str = str.Replace('-', '+').Replace('_', '/');
            switch (str.Length % 4)
            {
                case 2: str += "=="; break;
                case 3: str += "="; break;
            }
            return str;
        }
    }
}