using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Person
    {
        [JsonProperty("id")]
        public string PersonId { get; set; }

        public string Email { get; set; }

        public string  DisplayName { get; set; }

        [JsonIgnore]
        public List<Snippet> Snippets { get; set; }

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
    }
}
