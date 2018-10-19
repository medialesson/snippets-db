using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snippets.Web.Domains
{
    public class Category
    {
        /// <summary>
        /// unique identifier of the Category
        /// </summary>
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Name of the Category that should be displayed to the user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Background color of the Category that should be displayed to the user
        /// </summary>
        public string Color { get; set; } = "#000";

        /// <summary>
        /// Pivot table for linking the relationship between Snippets and its Categories
        /// </summary>
        public List<SnippetCategory> SnippetCategories { get; set; }
    }
}
