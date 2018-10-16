using Newtonsoft.Json;
using Snippets.Web.Features.Karma.Enums;

namespace Snippets.Web.Features.Karma
{
    public class Vote
    {
        [JsonIgnore]
        public VoteStatus Status { get; set; }

        [JsonProperty("status")]
        public string StatusString => Status.ToString().ToLower();
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