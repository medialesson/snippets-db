using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Person
    {
        public string PersonId { get; set; }

        public string Email { get; set; }

        public string  DisplayName { get; set; }

        [NotMapped]
        public int Score => Snippets?.Where(x => x.Author.PersonId == PersonId).Sum(s => s.Score) ?? 0;

        public List<Snippet> Snippets { get; set; }

        public List<Karma> Karma { get; set; }

        [NotMapped]
        public UserPreferences Preferences { get; set; } = new UserPreferences();
        
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
