using System.Text.Json.Serialization;

namespace Volte.Systems.Database.Entities;

public sealed class GuildData
{
    public static GuildData CreateFrom(IGuild guild)
        => new()
        {
            Id = guild.Id,
            OwnerId = guild.OwnerId,
            Configuration = new()
            {
                Autorole = default,
                CommandPrefix = Config.CommandPrefix,
                Moderation = new()
                {
                    AdminRole = default,
                    ModActionLogChannel = default,
                    ModRole = default,
                    CheckAccountAge = false,
                    VerifiedRole = default,
                    UnverifiedRole = default,
                    ShowResponsibleModerator = true
                },
                Welcome = new()
                {
                    LeavingMessage = string.Empty,
                    WelcomeChannel = default,
                    WelcomeColor = new Color(0x7000FB).RawValue,
                    WelcomeMessage = string.Empty
                }
            },
            Extras = new()
            {
                StarscriptTables = new StarscriptTables(),
                ModActionCaseNumber = default,
                SelfRoles = [],
                Tags = [],
                Warns = []
            }
        };

    public GuildData()
    {
        Configuration = new();
        Extras = new();
    }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("owner")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("configuration")]
    public GuildConfiguration Configuration { get; set; }

    [JsonPropertyName("extras")]
    public GuildExtras Extras { get; set; }

    public override string ToString()
        => JsonSerializer.Serialize(this, Config.JsonOptions);
}