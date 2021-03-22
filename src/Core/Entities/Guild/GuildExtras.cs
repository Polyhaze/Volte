using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Volte.Core.Entities
{
    public sealed class GuildExtras
    {
        internal GuildExtras()
        {
            SelfRoles = new List<string>();
            Tags = new List<Tag>();
            Warns = new List<Warn>();
        }

        [JsonPropertyName("self_roles")]
        public List<string> SelfRoles { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }

        [JsonPropertyName("warns")]
        public List<Warn> Warns { get; set; }

        [JsonPropertyName("mod_log_case_number")]
        public ulong ModActionCaseNumber { get; set; }
        
        [JsonPropertyName("auto_parse_quote_urls")]
        public bool AutoParseQuoteUrls { get; set; }

        public void AddWarn(Action<Warn> initializer)
        {
            var w = new Warn();
            initializer(w);
            Warns.Add(w);
        }
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}