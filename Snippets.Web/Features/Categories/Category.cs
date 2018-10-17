using System.Collections.Generic;

namespace Snippets.Web.Features.Categories
{
    public class Category
    {
        public string CategoryId { get; set; }

        public string DisplayName { get; set; }

        public string Color { get; set; }
    }

    public class CategoryEnvelope
    {
        public Category Category { get; set; }
    }

    public class CategoriesEnvelope
    {
        public IList<Category> Categories { get; set; }
        public int CategoriesCount { get; set; }
    }
}
