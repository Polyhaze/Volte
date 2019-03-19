using Discord.WebSocket;
using Qmmands;
using Volte.Commands;
using Volte.Services;

namespace Volte.Data.Objects
{
    public sealed class EvalObjects
    {
        public VolteContext Context { get; set; }
        public DiscordSocketClient Client { get; set; }
        public DiscordServer Config { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public DebugService DebugService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public EmojiService EmojiService { get; set; }
    }
}