using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Helpers;
using Volte.Core.Services;
using CommandService = Qmmands.CommandService;

namespace Volte.Core.Commands
{
    public abstract class VolteModule : Qmmands.ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public CommandService Cs { get; set; }
        public EmojiService RawEmoji { get; set; }
        public LoggingService Logger { get; set; }

        protected Task React(SocketUserMessage m, string r)
        {
            return Utils.React(m, r);
        }
    }
}