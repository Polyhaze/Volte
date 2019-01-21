using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Runtime;
using SIVA.Helpers;

namespace SIVA.Core.Discord {
    
    public class SIVAContext : ICommandContext {
        public SIVAContext(DiscordSocketClient client, IUserMessage msg) {
            Client = client;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            SMessage = msg as SocketUserMessage;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketUser;
            Message = msg;
        }
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public SocketUserMessage SMessage { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }
    }
}