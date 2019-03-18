using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Volte.Services;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands
{
    public sealed class VolteContext : ICommandContext
    {
        private readonly EmojiService _emojiService;

        public VolteContext(DiscordClient client, DiscordMessage msg, IServiceProvider provider)
        {
            _emojiService = provider.GetRequiredService<EmojiService>();
            Client = client;
            ServiceProvider = provider;
            Guild = msg.Channel.Guild;
            Channel = msg.Channel;
            User = msg.Author as DiscordMember;
            Message = msg;
        }

        public DiscordClient Client { get; }
        public IServiceProvider ServiceProvider { get; }
        public DiscordGuild Guild { get; }
        public DiscordChannel Channel { get; }
        public DiscordMember User { get; }
        public DiscordMessage Message { get; }

        public Task ReactFailureAsync() => Message.CreateReactionAsync(DiscordEmoji.FromUnicode(_emojiService.X));

        public Task ReactSuccessAsync() =>
            Message.CreateReactionAsync(DiscordEmoji.FromUnicode(_emojiService.BALLOT_BOX_WITH_CHECK));

        public DiscordEmbed CreateEmbed(string content) => new DiscordEmbedBuilder().WithSuccessColor().WithAuthor(User)
            .WithDescription(content).Build();

        public DiscordEmbedBuilder CreateEmbedBuilder(string content = null) => new DiscordEmbedBuilder()
            .WithSuccessColor().WithAuthor(User).WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);
        public Task ReplyAsync(DiscordEmbed embed) => embed.SendToAsync(Channel);
        public Task ReplyAsync(DiscordEmbedBuilder embed) => embed.SendToAsync(Channel);
        public Task ReactAsync(string unicode) => Message.CreateReactionAsync(DiscordEmoji.FromUnicode(unicode));
    }
}