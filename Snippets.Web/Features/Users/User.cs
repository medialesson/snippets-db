﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Snippets.Web.Features.Users
{
    public class User
    {
        /// <summary>
        /// Unique identifier of the User
        /// </summary>
        [JsonProperty("id")]
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
        /// Tokens valid for the User, used for authentication verification
        /// </summary>
        public UserTokens Tokens { get; set; }
    }

    public class UserTokens
    {
        /// <summary>
        /// Jwt token used for authentication
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh token used for generating a new Jwt token
        /// </summary>
        public string Refresh { get; set; }

#if DEBUG
        // Used for easier authentication within Swagger
        public string Debug => "Bearer " + Token;
#endif
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

    public class UserTokensEnvelope
    {
        /// <summary>
        /// Initializes a UserTokensEnvelope
        /// </summary>
        /// <param name="tokens">Instace of a UserTokens transfer object</param>
        public UserTokensEnvelope(UserTokens tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Instace of a UserTokens transfer object
        /// </summary>
        public UserTokens Tokens { get; }
    }
}
