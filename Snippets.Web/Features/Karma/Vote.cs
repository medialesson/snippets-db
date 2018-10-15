namespace Snippets.Web.Features.Karma
{
    public class Vote
    {
        
        public bool Status { get; set; }
    }

    public class VoteEnvelope
    {
        public VoteEnvelope(Vote vote)
        {
            Vote = vote;
        }

        public Vote Vote { get; set; }
    }
}