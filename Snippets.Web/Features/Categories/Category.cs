using System.Collections.Generic;

namespace Snippets.Web.Features.Categories
{
    public class Category
    {
        /// <summary>
        /// Unique identifier of the Category
        /// </summary>
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

    public class CategoryEnvelope
    {
        /// <summary>
        /// Initializes a CategoryEnvelope
        /// </summary>
        /// <param name="category">Instance of a Category tranfer object</param>
        public CategoryEnvelope(Category category)
        {
           Category = category;
        }

        /// <summary>
        /// Instance of a Category tranfer object
        /// </summary>
        public Category Category { get; }
    }

    public class CategoriesEnvelope
    {
        /// <summary>
        /// Initializes a CategoriesEnvelope
        /// </summary>
        /// <param name="categories">List if Category transfer objects</param>
        public CategoriesEnvelope(Listy<Category> categories)
        {
            Categories = categories;
        }

        /// <summary>
        /// List if Category transfer objects
        /// </summary>
        public IList<Category> Categories { get; }

        /// <summary>
        /// Number of listed categories
        /// </summary>
        public int CategoriesCount => Categories.Count();
    }
}
