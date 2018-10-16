using Snippets.Web.Domains;
using System.Collections;
using System.Collections.Generic;

namespace Snippets.Web.Features.Categories
{
    public class CategoriesEnvelope
    {
        public IList<Category> Categories { get; set; }
        public int CategoriesCount { get; set; }
    }
}