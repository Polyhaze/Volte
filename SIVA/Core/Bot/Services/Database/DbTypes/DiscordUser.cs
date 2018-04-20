using System;

namespace SIVA.Core.Bot.Services.Database.DbTypes
{
    public class DiscordUser : DatabaseEntity
    {
        public ulong Id { get; set; }

        public uint Xp { get; set; }

        public uint LevelNumber => (uint)Math.Sqrt(Xp / 50);

        public int Money { get; set; }
    }
}