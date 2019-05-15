using Discord.WebSocket;
using Qmmands;
using Volte.Commands;
using Volte.Services;

namespace Volte.Data.Models
{
    public sealed class EvalObjects
    {
        public VolteContext Context { get; set; }
        public DiscordSocketClient Client { get; set; }
        public GuildConfiguration Config { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public EmojiService EmojiService { get; set; }
    }
}