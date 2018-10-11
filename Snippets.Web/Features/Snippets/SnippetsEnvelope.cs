using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class SnippetsEnvelope
    {
        public ICollection<Snippet> Snippets { get; set; }
        public int SnippetsCount { get; set; }
    }
}
