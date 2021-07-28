using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;

namespace Volte.Core.Entities
{
    public sealed class GuildData
    {
        public static GuildData CreateFrom(IGuild guild)
            => new GuildData
            {
                Id = guild.Id,
                OwnerId = guild.OwnerId,
                Configuration = new GuildConfiguration
                {
                    Autorole = default,
                    CommandPrefix = Config.CommandPrefix,
                    Moderation = new ModerationOptions
                    {
                        AdminRole = default,
                        Antilink = default,
                        Blacklist = new HashSet<string>(),
                        MassPingChecks = default,
                        ModActionLogChannel = default,
                        ModRole = default,
                        CheckAccountAge = false,
                        VerifiedRole = default,
                        UnverifiedRole = default,
                        BlacklistAction = BlacklistAction.Nothing,
                        ShowResponsibleModerator = true
                    },
                    Welcome = new WelcomeOptions
                    {
                        LeavingMessage = string.Empty,
                        WelcomeChannel = default,
                        WelcomeColor = new Color(0x7000FB).RawValue,
                        WelcomeMessage = string.Empty
                    }
                },
                Extras = new GuildExtras
                {
                    ModActionCaseNumber = default,
                    SelfRoles = new HashSet<string>(),
                    Tags = new HashSet<Tag>(),
                    Warns = new HashSet<Warn>()
                }
            };

        public GuildData()
        {
            Configuration = new GuildConfiguration();
            Extras = new GuildExtras();
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
}