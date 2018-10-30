using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Domains
{
    public class Person
    {
        /// <summary>
        /// Unique identifier of the Person
        /// </summary>
        public string PersonId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Email address associated with the Person (optional)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Name of the Peron that should be displayed to other users (optional)
        /// </summary>
        public string  DisplayName { get; set; }

        /// <summary>
        /// Score of the Karma the Person received on all Snippets
        /// </summary>
        [NotMapped]
        public int Score => Snippets?.Where(x => x.Author.PersonId == PersonId).Sum(s => s.Score) ?? 0;

        /// <summary>
        /// Snippets the Person has published
        /// </summary>
        public List<Snippet> Snippets { get; set; }

        /// <summary>
        /// Karma the Person has submitted
        /// </summary>
        public List<Karma> Karma { get; set; }

        /// <summary>
        /// User-customizable snd specific preferences (see <see cref="UserPreferences" /> for further detail)
        /// </summary>
        [NotMapped]
        public UserPreferences Preferences { get; set; } = new UserPreferences();

        /// <summary>
        /// Key that is being sent for email verification
        /// </summary>
        public string VerificationKey { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Hashed password in binary form, used for verification
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Salt in binary form, used for securing the Persons PasswordHash
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        public string RefreshToken { get; set; }
    }
}
