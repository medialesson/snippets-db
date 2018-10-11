using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class SnippetCategory
    {
        public string SnippetId { get; set; }
        public Snippet Snippet { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
