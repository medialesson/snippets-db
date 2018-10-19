using Newtonsoft.Json;
using Snippets.Web.Features.Karma.Enums;

namespace Snippets.Web.Features.Karma
{
    public class Vote
    {
        /// <summary>
        /// Status a vote can represent
        /// </summary>
        [JsonIgnore]
        public VoteStatus Status { get; set; }

        /// <summary>
        /// Status of the vote that gets displayed in the response
        /// </summary>
        [JsonProperty("status")]
        public string StatusString => Status.ToString().ToLower();
    }

    public class VoteEnvelope
    {
        /// <summary>
        /// Initializes a VoteEnvelope
        /// </summary>
        /// <param name="vote">Instance of a Vote transfer object</param>
        public VoteEnvelope(Vote vote)
        {
            Vote = vote;
        }

        /// <summary>
        /// Instance of a Vote transfer object
        /// </summary>
        public Vote Vote { get; }
    }
}