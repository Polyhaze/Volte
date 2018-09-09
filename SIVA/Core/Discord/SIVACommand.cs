using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Runtime;
using SIVA.Helpers;

namespace SIVA.Core.Discord {
    
    public abstract class SIVACommand : ModuleBase<SocketCommandContext> {
        protected static Log GetLogger() => new Log();
        protected static Embed CreateEmbed(SocketCommandContext ctx, object desc) => Utils.CreateEmbed(ctx, desc);
        protected async Task Reply(ISocketMessageChannel c, Embed e) => await Utils.Send(c, e);
        protected async Task Reply(ISocketMessageChannel c, string m) => await Utils.Send(c, m);
    }
}