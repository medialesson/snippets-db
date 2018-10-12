using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class UserPreferences
    {
        // [JsonIgnore]
        // public string Id { get; set; }
        
        [JsonIgnore]
        public Person User { get; set; }

        [Key]
        [JsonIgnore]
        public string UserId { get; set; }

        public bool IsProfilePublic { get; set; }
    }
}
