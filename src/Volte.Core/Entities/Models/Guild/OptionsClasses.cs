using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Core.Entities
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
        public long WelcomeColor { get; set; }

        [JsonPropertyName("welcome_dm_message")]
        public string WelcomeDmMessage { get; set; }

        public string FormatWelcomeMessage(DiscordMember member)
            => FormatMessage(WelcomeMessage, member);

        public static string FormatMessage(string message, DiscordMember member) 
            => message.ReplaceIgnoreCase("{GuildName}", member.Guild.Name)
                .ReplaceIgnoreCase("{GuildName}", member.Guild.Name)
                .ReplaceIgnoreCase("{MemberName}", member.Username)
                .ReplaceIgnoreCase("{MemberMention}", member.Mention)
                .ReplaceIgnoreCase("{OwnerMention}", member.Guild.Owner.Mention)
                .ReplaceIgnoreCase("{MemberTag}", member.Discriminator)
                .ReplaceIgnoreCase("{MemberCount}", member.Guild.MemberCount)
                .ReplaceIgnoreCase("{MemberString}", member.AsPrettyString());

        public string FormatLeavingMessage(DiscordMember member)
            => FormatMessage(LeavingMessage, member);

        public string FormatDmMessage(DiscordMember member)
            => FormatMessage(WelcomeDmMessage, member);
        
        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }

    public sealed class StarboardOptions
    {
        [JsonPropertyName("starboard_channel")]
        public ulong StarboardChannel { get; set; }
        
        [JsonPropertyName("starboard_enabled")]
        public bool Enabled { get; set; }
        
        [JsonPropertyName("number_of_required_stars")]
        public int StarsRequiredToPost { get; set; }
        
        [JsonPropertyName("delete_invalid_stars")]
        public bool DeleteInvalidStars { get; set; }
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

    public enum NotificationType
    {
        MentionEveryone,
        MentionHere,
        Nothing
    }
}