using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Helpers;
using Volte.Core.Services;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands
{
    /// <inheritdoc />
    public class VolteContext : ICommandContext
    {
        private readonly EmojiService _emojiService = VolteBot.GetRequiredService<EmojiService>();

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

        public Task ReactFailureAsync() => Message.AddReactionAsync(new Emoji(_emojiService.X));
        public Task ReactSuccessAsync() => Message.AddReactionAsync(new Emoji(_emojiService.BALLOT_BOX_WITH_CHECK));
        public Embed CreateEmbed(string content) => Utils.CreateEmbed(this, content);
        public EmbedBuilder CreateEmbedBuilder(string content) => Utils.CreateEmbed(this, content).ToEmbedBuilder();
        public EmbedBuilder CreateEmbedBuilder() => Utils.CreateEmbed(this, string.Empty).ToEmbedBuilder();
        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);
        public Task ReplyAsync(Embed embed) => embed.SendTo(Channel);
        public Task ReplyAsync(EmbedBuilder embed) => embed.SendTo(Channel);
        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}