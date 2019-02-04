using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Runtime;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Commands {
    
    public class VolteContext : ICommandContext {
        public VolteContext(IDiscordClient client, IUserMessage msg) {
            Client = client;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            SMessage = msg as SocketUserMessage;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketUser;
            Message = msg;
            GuildUser = msg.Author as IGuildUser;
        }

        private readonly EmojiService _emojiService = VolteBot.ServiceProvider.GetRequiredService<EmojiService>();
        public Task ReactFailure() => SMessage.AddReactionAsync(new Emoji(_emojiService.X));
        public Task ReactSuccess() => SMessage.AddReactionAsync(new Emoji(_emojiService.BALLOT_BOX_WITH_CHECK));
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public SocketUserMessage SMessage { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }
        public IGuildUser GuildUser { get; }
    }
}