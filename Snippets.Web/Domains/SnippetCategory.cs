namespace Snippets.Web.Domains
{
    public class SnippetCategory
    {
        /// <summary>
        /// Unique identifier of the Snippet the Category is relatied to
        /// </summary>
        public string SnippetId { get; set; }

        /// <summary>
        /// Instance of the related Snippet
        /// </summary>
        public Snippet Snippet { get; set; }

        /// <summary>
        /// Unique identifier of the Category the Snippet is Linked to
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Instance of the related Category
        /// </summary>
        public Category Category { get; set; }
    }
}
