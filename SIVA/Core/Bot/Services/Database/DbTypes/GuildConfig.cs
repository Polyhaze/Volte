using System.Collections.Generic;

namespace SIVA.Core.Bot.Services.Database.DbTypes
{
    public class GuildConfig : DatabaseEntity
    {
        public GuildConfig()
        {
            AntilinkIgnoredChannels = new List<ulong>();
            SelfRoles = new List<string>();
            Blacklist = new List<string>();
            CustomCommands = new Dictionary<string, string>();
        }

        public ulong ServerId { get; set; }
        public bool CanCloseOwnTicket { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string SupportChannelName { get; set; }
        public ulong SupportChannelId { get; set; }
        public string SupportRole { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public bool Leveling { get; set; }
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColour1 { get; set; }
        public int WelcomeColour2 { get; set; }
        public int WelcomeColour3 { get; set; }
        public int EmbedColour1 { get; set; }
        public int EmbedColour2 { get; set; }
        public int EmbedColour3 { get; set; }
        public bool MassPengChecks { get; set; }
        public bool Antilink { get; set; }
        public bool VerifiedGuild { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public bool IsTodEnabled { get; set; }
        public ulong ServerLoggingChannel { get; set; }
        public bool IsServerLoggingEnabled { get; set; }
        public List<ulong> AntilinkIgnoredChannels { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<string> Blacklist { get; set; }
        public Dictionary<string, string> CustomCommands { get; set; }
    }
}