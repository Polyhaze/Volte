using System.Collections.Generic;
using Newtonsoft.Json;

namespace Volte.Core.Data.Models.Guild
{
    public sealed class GuildExtras
    {
        internal GuildExtras()
        {
            SelfRoles = new List<string>();
            Tags = new List<Tag>();
            Warns = new List<Warn>();
        }

        [JsonProperty("self_roles")]
        public List<string> SelfRoles { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }

        [JsonProperty("warns")]
        public List<Warn> Warns { get; set; }

        [JsonProperty("mod_log_case_number")]
        public ulong ModActionCaseNumber { get; set; }
    }
}