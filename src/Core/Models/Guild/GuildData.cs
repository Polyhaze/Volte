using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.WebSocket;

namespace Volte.Core.Models.Guild
{
    public sealed class GuildData
    {
        public GuildData(SocketGuild guild)
        {
            Configuration = new GuildConfiguration();
            Extras = new GuildExtras();
            Id = guild.Id;
            OwnerId = guild.OwnerId;
            Configuration = new GuildConfiguration
            {
                Autorole = default,
                CommandPrefix = Config.CommandPrefix,
                DeleteMessageOnCommand = default,
                Moderation = new ModerationOptions
                {
                    AdminRole = default,
                    Antilink = default,
                    Blacklist = new List<string>(),
                    MassPingChecks = default,
                    ModActionLogChannel = default,
                    ModRole = default
                },
                Welcome = new WelcomeOptions
                {
                    LeavingMessage = string.Empty,
                    WelcomeChannel = default,
                    WelcomeColor = new Color(0x7000FB).RawValue,
                    WelcomeMessage = string.Empty
                }
            };
            Extras = new GuildExtras
            {
                ModActionCaseNumber = default,
                SelfRoles = new List<string>(),
                Tags = new List<Tag>(),
                Warns = new List<Warn>()
            };

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
            => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
    }
}