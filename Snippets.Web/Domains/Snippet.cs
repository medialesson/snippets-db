using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Snippets.Web.Domains
{
    public class Snippet
    {
        [JsonProperty("id")]
        public string SnippetId { get; set; }

        public int Karma { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }

        [NotMapped]
        public List<Category> Categories => SnippetCategories?.Select(x => x.Category).ToList();
            
        [JsonIgnore]
        public List<SnippetCategory> SnippetCategories { get; set; }
    }
}
