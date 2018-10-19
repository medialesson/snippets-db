using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    public class User
    {
        /// <summary>
        /// Unique identifier of the User
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Email address associated with the Person (optional)
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// Name of the Peron that should be displayed to other users (optional)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Score of the Karma the Person received on all Snippets
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Jwt token valid for the User, used for authentication verification
        /// </summary>
        /// <value></value>
        public string Token { get; set; }

        public UserTokens Tokens { get; set; }

#if DEBUG
        // Used for easier authentication within Swagger
        public string DebugToken => "Bearer " + Token;
#endif
    }

    public class UserTokens {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }

    public class UserEnvelope
    {
        /// <summary>
        /// Initializes a UserEnvelope
        /// </summary>
        /// <param name="user">Instance of a User transfer object</param>
        public UserEnvelope(User user)
        {
            User = user;
        }

        /// <summary>
        /// Instance of a User transfer object
        /// </summary>
        public User User { get; }
    }
}
