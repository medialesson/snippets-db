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
        public string SnippetId { get; set; } = Guid.NewGuid().ToString();

        [NotMapped]
        [JsonIgnore]
        public List<Karma> Upvotes => Karma?.Where(x =>  x.Upvote).ToList() ?? new List<Karma>();

        [NotMapped]
        [JsonIgnore]
        public List<Karma> Downvotes => Karma?.Where(x => !x.Upvote).ToList() ?? new List<Karma>();

        [NotMapped]
        public int Score => Upvotes.Count() - Downvotes.Count();

        [JsonIgnore]
        public List<Karma> Karma { get; set; }

        public Person Author { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Language { get; set; }

        [NotMapped]
        public List<Category> Categories => SnippetCategories?.Select(x => x.Category).ToList();
            
        [JsonIgnore]
        public List<SnippetCategory> SnippetCategories { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
