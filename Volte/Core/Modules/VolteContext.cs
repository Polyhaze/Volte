using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Runtime;
using Volte.Helpers;

namespace Volte.Core.Modules {
    
    public class VolteContext : ICommandContext {
        public VolteContext(DiscordSocketClient client, IUserMessage msg) {
            Client = client;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            SMessage = msg as SocketUserMessage;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketUser;
            Message = msg;
            GuildUser = msg.Author as IGuildUser;
        }
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public SocketUserMessage SMessage { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }
        public IGuildUser GuildUser { get; }
    }
}