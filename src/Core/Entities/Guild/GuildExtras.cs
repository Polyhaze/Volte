using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public void AddTag(TagInitializer initializer)
        {
            var t = new Tag();
            initializer(t);
            Tags.Add(t);
        }

        public void AddWarn(WarnInitializer initializer)
        {
            var w = new Warn();
            initializer(w);
            Warns.Add(w);
        }
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}