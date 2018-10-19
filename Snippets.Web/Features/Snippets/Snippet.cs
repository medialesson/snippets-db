using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Snippets.Web.Domains;
using Snippets.Web.Features.Snippets.Enums;

namespace Snippets.Web.Features.Snippets
{
    public class Snippet
    {
        /// <summary>
        /// Unique identifier of the Snippet
        /// </summary>
        public string SnippetId { get; set; }

        /// <summary>
        /// Title of the Snippet
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content of the Snippet (usually code) 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Score of negative and positive Karma the Snippet received
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Person that submitted the Snippet
        /// </summary>
        public SnippetAuthor Author { get; set; }

        /// <summary>
        /// Programming language the Snippets content is in
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// List of all Categories that belong to the Snippet
        /// </summary>
        public List<SnippetCategory> Categories { get; set; }

        /// <summary>
        /// Timestamp of the time the Snippet has been created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestam of the last time the Snippet has been updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    public class SnippetAuthor
    {
        /// <summary>
        /// Unique identifier of the Author submitting the Snippet
        /// </summary>
        public string AuthorId { get; set; }

        /// <summary>
        /// Name of the Peron submitting the Snippet 
        /// </summary>
        public string DisplayName { get; set; }
    }


    public class SnippetCategory
    {
        /// <summary>
        /// Unique identifier of the Category
        /// </summary>
        [JsonProperty("id")]
        public string CategoryId { get; set; } 

        /// <summary>
        /// Name of the Category that should be displayed to the user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Background color of the Category that should be displayed to the user
        /// </summary>
        public string Color { get; set; } 
    }

    public class SnippetEnvelope
    {
        /// <summary>
        /// Initializes a SnippetEnvelope
        /// </summary>
        /// <param name="snippet">Instance of a Snippet transfer object</param>
        public SnippetEnvelope(Snippet snippet)
        {
            Snippet = snippet;
        }

        /// <summary>
        /// Instance of a Snippet transfer object
        /// </summary>
        public Snippet Snippet { get; }
    }

    public class SnippetsEnvelope
    {
        /// <summary>
        /// Initializes a SnippetsEnvelope
        /// </summary>
        /// <param name="snippets">List of Snippet transfer objects</param>
        public SnippetsEnvelope(List<Snippet> snippets = null)
        {
            Snippets = snippets;
        }

        /// <summary>
        /// List of Snippet transfer objects 
        /// </summary>
        public List<Snippet> Snippets { get; } = new List<Snippet>();

        /// <summary>
        /// Number of listed snippets
        /// </summary>
        [JsonProperty("count")] 
        public int SnippetsCount => Snippets.Count;
    }    
}
