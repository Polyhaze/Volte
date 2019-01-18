using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Runtime;
using SIVA.Helpers;

namespace SIVA.Core.Discord {
    public abstract class SIVACommand : ModuleBase<SIVAContext> {
        protected static Log GetLogger() => new Log();
        protected static Embed CreateEmbed(SIVAContext ctx, object desc) => Utils.CreateEmbed(ctx, desc);
        protected async Task Reply(IMessageChannel c, Embed e) => await Utils.Send(c, e);
        protected async Task Reply(IMessageChannel c, string m) => await Utils.Send(c, m);
        protected async Task React(SocketUserMessage m, string r) => await Utils.React(m, r);
    }
}