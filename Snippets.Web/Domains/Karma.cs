namespace Snippets.Web.Domains
{
    public class Karma
    {
        /// <summary>
        /// The unique identifier of the Karma 
        /// </summary>
        public string KarmaId { get; set; }

        /// <summary>
        /// Whether the Karma is a positive vote
        /// </summary>
        public bool Upvote { get; set; }

        /// <summary>
        /// The Snippet the Karma vote is related to
        /// </summary>
        public Snippet Snippet { get; set; }

        /// <summary>
        ///  The Person that submitted the Karma vote
        /// </summary>
        public Person Submitter { get; set; }
    }
}