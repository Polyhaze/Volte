using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gommon;

namespace Volte.Core.Entities
{
    public sealed class GuildExtras
    {
        internal GuildExtras()
        {
            SelfRoles = new HashSet<string>();
            Tags = new HashSet<Tag>();
            Warns = new HashSet<Warn>();
        }
        
        [JsonPropertyName("mod_log_case_number")]
        public ulong ModActionCaseNumber { get; set; }
        
        [JsonPropertyName("auto_parse_quote_urls")]
        public bool AutoParseQuoteUrls { get; set; }

        [JsonPropertyName("self_roles")]
        public HashSet<string> SelfRoles { get; set; }

        [JsonPropertyName("tags")]
        public HashSet<Tag> Tags { get; set; }

        [JsonPropertyName("warns")]
        public HashSet<Warn> Warns { get; set; }

        public void AddTag(TagInitializer initializer) => Tags.Add(new Tag().Apply(t => initializer(t)));
        public void AddWarn(WarnInitializer initializer) => Warns.Add(new Warn().Apply(w => initializer(w)));
        
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}