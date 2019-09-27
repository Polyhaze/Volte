using System.Collections.Generic;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Volte.Core.Models.Guild
{
    public sealed class ModerationOptions
    {
        internal ModerationOptions()
            => Blacklist = new List<string>();

        [JsonProperty("mass_ping_checks")]
        public bool MassPingChecks { get; set; }

        [JsonProperty("antilink")]
        public bool Antilink { get; set; }

        [JsonProperty("mod_log_channel")]
        public ulong ModActionLogChannel { get; set; }

        [JsonProperty("mod_role")]
        public ulong ModRole { get; set; }

        [JsonProperty("admin_role")]
        public ulong AdminRole { get; set; }

        [JsonProperty("blacklist")]
        public List<string> Blacklist { get; set; }
    }

    public sealed class WelcomeOptions
    {

        [JsonProperty("welcome_channel")]
        public ulong WelcomeChannel { get; set; }

        [JsonProperty("welcome_message")]
        public string WelcomeMessage { get; set; }

        [JsonProperty("leaving_message")]
        public string LeavingMessage { get; set; }

        [JsonProperty("welcome_color")]
        public uint WelcomeColor { get; set; }

        [JsonProperty("welcome_dm_message")]
        public string WelcomeDmMessage { get; set; }

        public string FormatWelcomeMessage(SocketGuildUser user) 
            => WelcomeMessage.Replace("{ServerName}", user.Guild.Name)
                .Replace("{GuildName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator)
                .Replace("{MemberCount}", user.Guild.MemberCount.ToString())
                .Replace("{UserString}", user.ToString());

        public string FormatLeavingMessage(SocketGuildUser user) 
            => LeavingMessage.Replace("{ServerName}", user.Guild.Name)
                .Replace("{GuildName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator)
                .Replace("{MemberCount}", user.Guild.MemberCount.ToString())
                .Replace("{UserString}", user.ToString());

        public string FormatDmMessage(SocketGuildUser user)
            => WelcomeDmMessage.Replace("{ServerName}", user.Guild.Name)
                .Replace("{GuildName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator)
                .Replace("{MemberCount}", user.Guild.MemberCount.ToString())
                .Replace("{UserString}", user.ToString());
    }
}