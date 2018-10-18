using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    public class User
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public int Score { get; set; }

        public string Token { get; set; }

#if DEBUG
        public string DebugToken => "Bearer " + Token;
#endif
    }

    public class UserEnvelope
    {
        public UserEnvelope(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}
