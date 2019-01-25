using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Runtime;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Modules {
    public abstract class VolteModule : ModuleBase<VolteContext> {
        public DatabaseService Db { get; set; }
        protected Log GetLogger() => new Log();
        protected Embed CreateEmbed(VolteContext ctx, object desc) => Utils.CreateEmbed(ctx, desc);
        protected async Task Reply(IMessageChannel c, Embed e) => await Utils.Send(c, e);
        protected async Task Reply(IMessageChannel c, string m) => await Utils.Send(c, m);
        protected async Task React(SocketUserMessage m, string r) => await Utils.React(m, r);
    }
}