using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gommon;
using Volte;

namespace Volte.Entities
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

        public void AddTag(Action<Tag> initializer) => Tags.Add(new Tag().Apply(initializer));
        public void AddWarn(Action<Warn> initializer) => Warns.Add(new Warn().Apply(initializer));
        
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}