using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.WebSocket;
using Gommon;

namespace Volte.Core.Models.Guild
{
    public sealed class ModerationOptions
    {
        internal ModerationOptions()
            => Blacklist = new List<string>();

        [JsonPropertyName("mass_ping_checks")]
        public bool MassPingChecks { get; set; }

        [JsonPropertyName("antilink")]
        public bool Antilink { get; set; }

        [JsonPropertyName("mod_log_channel")]
        public ulong ModActionLogChannel { get; set; }

        [JsonPropertyName("mod_role")]
        public ulong ModRole { get; set; }

        [JsonPropertyName("admin_role")]
        public ulong AdminRole { get; set; }
        
        [JsonPropertyName("check_account_age_on_join")]
        public bool CheckAccountAge { get; set; }

        [JsonPropertyName("blacklist")]
        public List<string> Blacklist { get; set; }

        [JsonPropertyName("blacklist_action")]
        public BlacklistAction BlacklistAction { get; set; }
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }

    public sealed class WelcomeOptions
    {
        [JsonPropertyName("welcome_channel")]
        public ulong WelcomeChannel { get; set; }

        [JsonPropertyName("welcome_message")]
        public string WelcomeMessage { get; set; }

        [JsonPropertyName("leaving_message")]
        public string LeavingMessage { get; set; }

        [JsonPropertyName("welcome_color")]
        public uint WelcomeColor { get; set; }

        [JsonPropertyName("welcome_dm_message")]
        public string WelcomeDmMessage { get; set; }

        public string FormatWelcomeMessage(SocketGuildUser user)
            => WelcomeMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Username)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);

        public string FormatLeavingMessage(SocketGuildUser user)
            => LeavingMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Username)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);

        public string FormatDmMessage(SocketGuildUser user)
            => WelcomeDmMessage.ReplaceIgnoreCase("{ServerName}", user.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", user.Guild.Name)
                .ReplaceIgnoreCase("{UserName}", user.Username)
                .ReplaceIgnoreCase("{UserMention}", user.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", user.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{UserTag}", user.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", user.Guild.MemberCount)
                .ReplaceIgnoreCase("{UserString}", user);
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }

    public static class BlacklistActions
    {
        public static BlacklistAction DetermineAction(string input)
            => input.ToLower() switch
            {
                "nothing" => BlacklistAction.Nothing,
                "warn" => BlacklistAction.Warn,
                "kick" => BlacklistAction.Kick,
                "ban" => BlacklistAction.Ban,
                _ => throw new NotSupportedException($"BlacklistAction {input.ToLower()} is not supported."),
            };
    }

    public enum BlacklistAction
    {
        Nothing,
        Warn,
        Kick,
        Ban

    }
}