using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Category
    {
        [JsonProperty("id")]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        public string DisplayName { get; set; }

        public string Color { get; set; } = "#000";

        [NotMapped]
        public List<string> Snippets => SnippetCategories?.Select(x => x.SnippetId).ToList();

        [JsonIgnore]
        public List<SnippetCategory> SnippetCategories { get; set; }
    }
}
