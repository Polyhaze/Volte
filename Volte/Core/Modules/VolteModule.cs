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
        public EmojiService RawEmoji { get; set; }
        protected Logger GetLogger() => new Logger();
        protected Embed CreateEmbed(VolteContext ctx, object desc) => Utils.CreateEmbed(ctx, desc);
        protected Task<IUserMessage> Reply(IMessageChannel c, Embed e) => Utils.Send(c, e);
        protected Task<IUserMessage> Reply(IMessageChannel c, string m) => Utils.Send(c, m);
        protected Task React(SocketUserMessage m, string r) => Utils.React(m, r);
    }
}