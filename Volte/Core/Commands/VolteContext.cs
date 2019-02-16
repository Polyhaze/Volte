using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Helpers;
using Volte.Core.Services;
using Qmmands;
using Volte.Core.Data.Objects;

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
            Guild = (msg.Channel as SocketTextChannel)?.Guild;
            Channel = msg.Channel as SocketTextChannel;
            User = msg.Author as SocketGuildUser;
            Message = msg as SocketUserMessage;
        }

        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public SocketTextChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }

        public Task ReactFailure() => Message.AddReactionAsync(new Emoji(_emojiService.X));

        public Task ReactSuccess() => Message.AddReactionAsync(new Emoji(_emojiService.BALLOT_BOX_WITH_CHECK));

        public Embed CreateEmbed(string content) => Utils.CreateEmbed(this, content);
        public EmbedBuilder CreateEmbedBuilder(string content) => Utils.CreateEmbed(this, content).ToEmbedBuilder();
    }
}