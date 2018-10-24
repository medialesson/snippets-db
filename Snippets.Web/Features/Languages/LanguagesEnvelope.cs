using System.Collections.Generic;
using Newtonsoft.Json;
using Snippets.Web.Features.Languages.Enums;

namespace Snippets.Web.Features.Languages
{
    public class LanguagesEnvelope
    {
        /// <summary>
        /// Initializes a LanguagesEnvelope
        /// </summary>
        /// <param name="languages">List of Language enums</param>
        public LanguagesEnvelope(string[] languages = null)
        {
            Languages = languages;
        }

        /// <summary>
        /// List of Language enums as string 
        /// </summary>
        public string[] Languages { get; } = new string[0];

        /// <summary>
        /// Number of listed languages
        /// </summary>
        [JsonProperty("count")] 
        public int LanguagesCount => Languages.Length;
    }
}