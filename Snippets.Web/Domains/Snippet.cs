using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Snippet
    {
        public string SnippetId { get; set; }

        public int Karma { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }

        [JsonIgnore]
        public List<Category> Categories { get; set; }
    }
}
