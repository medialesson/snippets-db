using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Snippets.Web.Domains.Enums;

namespace Snippets.Web.Domains
{
    public class Snippet
    {
        [JsonProperty("id")]
        public string SnippetId { get; set; } = Guid.NewGuid().ToString();

        // TODO: Add pivot table for karma assignment (person <-> karma)
        // [JsonIgnore] public SnippetKarma { get; set; }

        [NotMapped]
        public int Karma => 0;

        public Person Author { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; } = ProgrammingLanguage.Default;

        [NotMapped]
        public List<string> Categories => SnippetCategories?.Select(x => x.CategoryId).ToList();
            
        [JsonIgnore]
        public List<SnippetCategory> SnippetCategories { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
