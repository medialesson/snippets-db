namespace Snippets.Web.Domains
{
    public class SnippetKarma
    {
        public string SnippetId { get; set; }
        public Snippet Snippet { get; set; }

        public string KarmaId { get; set; }
        public Karma Karma { get; set; }
    }
}