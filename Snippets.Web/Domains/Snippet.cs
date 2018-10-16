using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Snippets.Web.Domains
{
    public class Snippet
    {
        public string SnippetId { get; set; } = Guid.NewGuid().ToString();

        [NotMapped]
        public List<Karma> Upvotes => Karma?.Where(x =>  x.Upvote).ToList() ?? new List<Karma>();

        [NotMapped]
        public List<Karma> Downvotes => Karma?.Where(x => !x.Upvote).ToList() ?? new List<Karma>();

        [NotMapped]
        public int Score => Upvotes.Count() - Downvotes.Count();

        public List<Karma> Karma { get; set; }

        public Person Author { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Language { get; set; }

        [NotMapped]
        public List<Category> Categories => SnippetCategories?.Select(x => x.Category).ToList();
            
        public List<SnippetCategory> SnippetCategories { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
