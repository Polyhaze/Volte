using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Gommon;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands
{
    public sealed class VolteContext : CommandContext
    {
        private readonly EmojiService _emojiService;

        public static VolteContext Create(SocketMessage msg, IServiceProvider provider) 
            => new VolteContext(msg, provider);

        // ReSharper disable once SuggestBaseTypeForParameter
        private VolteContext(SocketMessage msg, IServiceProvider provider)
        {
            provider.Get(out _emojiService);
            provider.Get<DatabaseService>(out var db);
            provider.Get(out Client);
            ServiceProvider = provider;
            Guild = msg.Channel.Cast<SocketTextChannel>()?.Guild;
            Channel = msg.Channel.Cast<SocketTextChannel>();
            User = msg.Author.Cast<SocketGuildUser>();
            Message = msg.Cast<SocketUserMessage>();
            GuildData = db.GetData(Guild);
            Now = DateTimeOffset.UtcNow;
        }

        public DiscordShardedClient Client;
        public IServiceProvider ServiceProvider;
        public SocketGuild Guild;
        public SocketTextChannel Channel;
        public SocketGuildUser User;
        public SocketUserMessage Message;
        public GuildData GuildData;
        public DateTimeOffset Now;

        public Task ReactFailureAsync() => Message.AddReactionAsync(new Emoji(_emojiService.X));

        public Task ReactSuccessAsync() => Message.AddReactionAsync(new Emoji(_emojiService.BallotBoxWithCheck));

        public Embed CreateEmbed(string content) => new EmbedBuilder().WithSuccessColor().WithAuthor(User)
            .WithDescription(content).Build();

        public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
            .WithSuccessColor().WithAuthor(User).WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);

        public Task ReplyAsync(Embed embed) => embed.SendToAsync(Channel);

        public Task ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}