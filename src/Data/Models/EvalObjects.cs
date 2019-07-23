using Discord.WebSocket;
using Qmmands;
using Volte.Commands;
using Volte.Data.Models.Guild;
using Volte.Services;

namespace Volte.Data.Models
{
    public sealed class EvalObjects
    {
        internal EvalObjects() { }

        public VolteContext Context { get; set; }
        public DiscordShardedClient Client { get; set; }
        public GuildData Data { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public BinService BinService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public EmojiService EmojiService { get; set; }
    }
}