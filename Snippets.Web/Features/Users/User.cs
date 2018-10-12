using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    public class User
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Token { get; set; }

        public UserPreferences Preferences { get; set; }
    }

    public class UserEnvelope
    {
        public UserEnvelope(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
