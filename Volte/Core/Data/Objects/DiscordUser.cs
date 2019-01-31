using System;
using LiteDB;

namespace Volte.Core.Data.Objects {
    public class DiscordUser : Object {
        public string Tag { get; internal set; }
        public ObjectId Id { get; set; }
        public ulong UserId { get; internal set; }
        public ulong Xp { get; internal set; }
        public uint Level => (uint)Math.Sqrt(Xp / 50);
        public int Money { get; set; }
    }
}