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

        [NotMapped]
        [JsonIgnore]
        public List<SnippetKarma> Upvotes => SnippetKarma.Where(x => x.Karma.Upvote).ToList();

        [NotMapped]
        [JsonIgnore]
        public List<SnippetKarma> Downvotes => SnippetKarma.Except(Upvotes).ToList();

        [NotMapped]
        public int Score => Upvotes.Count() - Downvotes.Count();

        [JsonIgnore]
        public List<SnippetKarma> SnippetKarma { get; set; }

        public Person Author { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public Language Language { get; set; } = Language.Plain;

        [NotMapped]
        public List<string> Categories => SnippetCategories?.Select(x => x.CategoryId).ToList();
            
        [JsonIgnore]
        public List<SnippetCategory> SnippetCategories { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
