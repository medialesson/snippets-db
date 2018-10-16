namespace Snippets.Web.Domains
{
    public class Karma
    {
        public string KarmaId { get; set; }

        public bool Upvote { get; set; }

        public Snippet Snippet { get; set; }

        public Person Submitter { get; set; }
    }
}