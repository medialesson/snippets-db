using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Category
    {
        public string CategoryId { get; set; }

        public string DisplayName { get; set; }

        public string Color { get; set; } = "#000";

        [JsonIgnore]
        public List<Snippet> Snippets { get; set; }
    }
}
