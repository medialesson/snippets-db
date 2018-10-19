using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Snippets.Web.Domains
{
    public class Snippet
    {
        /// <summary>
        /// Unique identifier of the Snippet
        /// </summary>
        public string SnippetId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// List of positive Karma votes the Snippet received only 
        /// </summary>
        [NotMapped]
        public List<Karma> Upvotes => Karma?.Where(x =>  x.Upvote).ToList() ?? new List<Karma>();

        /// <summary>
        /// List of negative Karma votes the Snippet received only
        /// </summary>
        [NotMapped]
        public List<Karma> Downvotes => Karma?.Where(x => !x.Upvote).ToList() ?? new List<Karma>();

        /// <summary>
        /// Score of negative and positive Karma the Snippet received
        /// </summary>
        [NotMapped]
        public int Score => Upvotes.Count() - Downvotes.Count();

        /// <summary>
        /// List of all Karma votes the Snipped received
        /// </summary>
        public List<Karma> Karma { get; set; }

        /// <summary>
        /// Person that submitted the Snippet
        /// </summary>
        public Person Author { get; set; }

        /// <summary>
        /// Title of the Snippet
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content of the Snippet (usually code) 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Programming language the Snippets content is in
        /// </summary>
        public int Language { get; set; }

        /// <summary>
        /// List of all Categories that belong to the Snippet
        /// </summary>
        [NotMapped]
        public List<Category> Categories => SnippetCategories?.Select(x => x.Category).ToList();
            
        /// <summary>
        /// Pivot table for linking the relationship between the Snippet and its Categoriesits Categories
        /// </summary>
        public List<SnippetCategory> SnippetCategories { get; set; }

        /// <summary>
        /// Timestamp of the time the Snippet has been created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestam of the last time the Snippet has been updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
