using LiteDB;

namespace Volte.Core.Data.Objects
{
    public class DiscordUser : object
    {
        public string Tag { get; internal set; }
        public ObjectId Id { get; set; }
        public ulong UserId { get; internal set; }
    }
}