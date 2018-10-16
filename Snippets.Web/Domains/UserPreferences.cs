using Newtonsoft.Json;

namespace Snippets.Web.Domains
{
    public class UserPreferences
    {
        [JsonIgnore]
        public bool IsProfilePublic { get; set; }
    }
}
