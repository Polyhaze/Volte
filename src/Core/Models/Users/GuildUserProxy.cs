using Discord;

namespace BrackeysBot.Core.Models
{
    public struct GuildUserProxy
    {
        public IGuildUser GuildUser { get; set; }
        public ulong ID { get; set; }

        public bool HasValue => GuildUser != null;
    }
}
