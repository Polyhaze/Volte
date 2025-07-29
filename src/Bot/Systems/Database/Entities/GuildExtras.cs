using System.Text.Json.Serialization;

namespace Volte.Systems.Database.Entities;

public sealed class GuildExtras
{
    [JsonPropertyName("mod_log_case_number")]
    public ulong ModActionCaseNumber { get; set; }
        
    [JsonPropertyName("auto_parse_quote_urls")]
    public bool AutoParseQuoteUrls { get; set; }

    [JsonPropertyName("self_roles")]
    public HashSet<ulong> SelfRoles { get; set; } = [];

    [JsonPropertyName("tags")]
    public HashSet<Tag> Tags { get; set; } = [];

    [JsonPropertyName("starscript_tables")]
    public StarscriptTables StarscriptTables { get; set; } = new();

    public void AddTag(Tag tag)
    {
        var existingIdenticalTag = Tags.FirstOrDefault(it => it.Response == tag.Response);
        if (existingIdenticalTag is not null)
        {
            Tags.Remove(existingIdenticalTag);
            existingIdenticalTag.Aliases.Add(tag.Name);
            Tags.Add(existingIdenticalTag);
        }
        else
        {
            Tags.Add(tag);
        }
    }

    [JsonPropertyName("warns")]
    public HashSet<Warn> Warns { get; set; } = [];

    public Gommon.Optional<Tag> GetTagByNameOrAlias(string nameOrAlias)
        => Tags.FindFirst(tag => tag.Name.EqualsIgnoreCase(nameOrAlias)
                                      || nameOrAlias.EqualsAnyIgnoreCase(tag.Aliases.ToArray())
        );

    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}