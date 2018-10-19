using Newtonsoft.Json;

namespace Snippets.Web.Domains
{
    public class UserPreferences
    {
        /// <summary>
        /// Whether the profile is visible for other users
        /// </summary>
        [JsonIgnore]
        public bool IsProfilePublic { get; set; }
    }
}
