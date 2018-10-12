using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class UserPreferences
    {
        public string Id { get; set; }

        [JsonIgnore]
        public Person User { get; set; }
        public string UserId { get; set; }

        public bool IsProfilePublic { get; set; }
    }
}
