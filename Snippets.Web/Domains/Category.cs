using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Category
    {
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        public string DisplayName { get; set; }

        public string Color { get; set; } = "#000";
        public List<SnippetCategory> SnippetCategories { get; set; }
    }
}
