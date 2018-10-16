using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Snippets.Web.Domains
{
    public class Karma
    {
        [JsonProperty("id")]
        public string KarmaId { get; set; }

        public bool Upvote { get; set; }

        [JsonIgnore]
        public Snippet Snippet { get; set; }

        [JsonIgnore]
        public Person Submitter { get; set; }
    }
}