using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class SnippetEnvelope
    {
        public SnippetEnvelope(Snippet snippet)
        {
            Snippet = snippet;
        }

        public Snippet Snippet { get; set; }
    }
}
