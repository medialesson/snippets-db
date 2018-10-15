using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public UserPreferences Preferences { get; set; } = new UserPreferences();

        [JsonIgnore]
        public List<Karma> Karma { get; set; }

        [NotMapped]
        public int Score => Snippets?.Where(x => x.Author.PersonId == PersonId).Sum(s => s.Score) ?? 0;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
    }
}
