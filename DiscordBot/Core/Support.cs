using Discord;

namespace DiscordBot.Core
{
    public class Support
    {
        public ulong ServerId { get; set; }
        public bool CanCloseOwnTicket { get; set; }
        public string ReactionEmoji { get; set; }
        public ulong SupportChannelId { get; set; }
        public string SupportChannelName { get; set; }
        public ulong SupportCategoryId { get; set; }
        public string SupportRole { get; set; }
    }
}
