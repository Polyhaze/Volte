using System;

namespace SIVA.Core.Bot.Services.Database.DbTypes
{
    public class DiscordUser : DatabaseEntity
    {
        public ulong UserId { get; set; }

        public uint Xp { get; set; }

        public uint LevelNumber => (uint)Math.Sqrt(Xp / 50);

        public long Money { get; set; }
    }
}