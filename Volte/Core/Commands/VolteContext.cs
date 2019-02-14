using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Services;
using Volte.Core.Helpers;

namespace Volte.Core.Commands {
    /// <inheritdoc />
    public class VolteContext : ICommandContext {
        public VolteContext(IDiscordClient client, IUserMessage msg) {
            Client = client as DiscordSocketClient;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketGuildUser;
            Message = msg as SocketUserMessage;
        }

        private readonly EmojiService _emojiService = VolteBot.ServiceProvider.GetRequiredService<EmojiService>();
        private readonly CommandService _commandService = VolteBot.ServiceProvider.GetRequiredService<CommandService>();
        public Task ReactFailure() => Message.AddReactionAsync(new Emoji(_emojiService.X));
        public Task ReactSuccess() => Message.AddReactionAsync(new Emoji(_emojiService.BALLOT_BOX_WITH_CHECK));
        public Embed CreateEmbed(string content) => Utils.CreateEmbed(this, content);
        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }
    }
}