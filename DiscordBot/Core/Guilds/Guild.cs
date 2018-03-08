namespace SIVA.Core.Config
{
    public class Guild
    {
        public ulong ServerId { get; set; }
        public bool CanCloseOwnTicket { get; set; }
        public string ReactionEmoji { get; set; }
        public ulong GuildOwnerId { get; set; }
        public ulong SupportChannelId { get; set; }
        public string SupportChannelName { get; set; }
        public ulong SupportCategoryId { get; set; }
        public string SupportRole { get; set; }
        public string RoleToApply { get; set; }
        public ulong ChannelId { get; set; }
        public string CommandPrefix { get; set; }
        public bool Leveling { get; set; }
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColour1 { get; set; }
        public int WelcomeColour2 { get; set; }
        public int WelcomeColour3 { get; set; }
        public bool MassPengChecks { get; set; }
        public int ModlogCase { get; set;}
        public bool Antilink { get; set; }
    }
}
