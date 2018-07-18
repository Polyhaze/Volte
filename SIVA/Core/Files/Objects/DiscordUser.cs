using System;

namespace SIVA.Core.Files.Objects
{
    public class DiscordUser : Object
    {
        public string Tag { get; internal set; }
        public ulong Id { get; internal set; }
        public ulong Xp { get; set; }
        public uint Level => (uint)Math.Sqrt(Xp/ 50);
        public int Money { get; set; }
    }
}