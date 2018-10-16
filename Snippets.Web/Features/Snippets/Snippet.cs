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
        public string SnippetId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Score { get; set; }

        public SnippetAuthor Author { get; set; }

        public Language Language { get; set; }

        public List<SnippetCategory> Categories { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class SnippetAuthor
    {
        public string AuthorId { get; set; }

        public string DisplayName { get; set; }
    }


    public class SnippetCategory
    {
        [JsonProperty("id")]
        public string CategoryId { get; set; } 

        public string DisplayName { get; set; }

        public string Color { get; set; } 
    }

    public class SnippetEnvelope
    {
        public SnippetEnvelope(Snippet snippet)
        {
            Snippet = snippet;
        }

        public Snippet Snippet { get; set; }
    }

    public class SnippetsEnvelope
    {
        public ICollection<Snippet> Snippets { get; set; }

        [JsonProperty("count")] 
        public int SnippetsCount { get; set; }
    }    
}
