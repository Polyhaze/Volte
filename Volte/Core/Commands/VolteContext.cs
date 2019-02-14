using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Helpers;
using Volte.Core.Services;
using CommandService = Qmmands.CommandService;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Core.Commands
{
    /// <inheritdoc />
    public class VolteContext : ICommandContext
    {
        private readonly CommandService _commandService = VolteBot.ServiceProvider.GetRequiredService<CommandService>();

        private readonly EmojiService _emojiService = VolteBot.ServiceProvider.GetRequiredService<EmojiService>();

        public VolteContext(IDiscordClient client, IUserMessage msg)
        {
            Client = client as DiscordSocketClient;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketGuildUser;
            Message = msg as SocketUserMessage;
        }

        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }

        public Task ReactFailure()
        {
            return Message.AddReactionAsync(new Emoji(_emojiService.X));
        }

        public Task ReactSuccess()
        {
            return Message.AddReactionAsync(new Emoji(_emojiService.BALLOT_BOX_WITH_CHECK));
        }

        public Embed CreateEmbed(string content)
        {
            return Utils.CreateEmbed(this, content);
        }
    }
}