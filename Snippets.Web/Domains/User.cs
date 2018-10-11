﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class User
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string  DisplayName { get; set; }

        [JsonIgnore]
        public List<Snippet> Snippets { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
