using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Volte.Core.Services;
using Volte.Core.Helpers;

namespace Volte.Core.Commands {
    public abstract class VolteModule : ModuleBase<VolteContext> {
        public DatabaseService Db { get; set; }
        public CommandService Cs { get; set; }
        public EmojiService RawEmoji { get; set; }
        public LoggingService Logger { get; set; }
        protected Task React(SocketUserMessage m, string r) => Utils.React(m, r);
    }
}