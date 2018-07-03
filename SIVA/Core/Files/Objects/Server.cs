using System.Collections.Generic;

namespace SIVA.Core.Files.Objects
{
    public class Server
    {
        #region JsonValueDeclaration

        public Server()
        {
            AntilinkIgnoredChannels = new List<ulong>();
            SelfRoles = new List<string>();
            Blacklist = new List<string>();
            CustomCommands = new Dictionary<string, string>();
            RandomRoles = new List<ulong>();
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
        public int WelcomeColourR { get; set; }
        public int WelcomeColourG { get; set; }
        public int WelcomeColourB { get; set; }
        public int EmbedColourR { get; set; }
        public int EmbedColourG { get; set; }
        public int EmbedColourB { get; set; }
        public bool MassPengChecks { get; set; }
        public bool Antilink { get; set; }
        public bool VerifiedGuild { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public bool IsTodEnabled { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public ulong ServerLoggingChannel { get; set; }
        public bool IsServerLoggingEnabled { get; set; }
        public List<ulong> AntilinkIgnoredChannels { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<string> Blacklist { get; set; }
        public List<ulong> RandomRoles { get; set; }
        public Dictionary<string, string> CustomCommands { get; set; }

        #endregion
    }
}