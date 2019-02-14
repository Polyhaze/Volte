using Discord;
using Discord.Commands;

namespace Volte.Core.Data.Objects
{
    public class Tag
    {
        public string Name { get; set; }
        public string Response { get; set; }
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public long Uses { get; set; }
    }
}