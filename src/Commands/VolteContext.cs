using System.Threading.Tasks;
using Discord;
 
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Models.Guild;
using Volte.Services;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Commands
{
    public sealed class VolteContext : ICommandContext
    {
        private readonly EmojiService _emojiService;

        // ReSharper disable once SuggestBaseTypeForParameter
        public VolteContext(DiscordShardedClient client, SocketUserMessage msg, ServiceProvider provider)
        {
            provider.Get(out _emojiService);
            provider.Get<DatabaseService>(out var db);
            Client = client;
            ServiceProvider = provider;
            Guild = msg.Channel.Cast<SocketTextChannel>()?.Guild;
            Channel = msg.Channel.Cast<SocketTextChannel>();
            User = msg.Author.Cast<SocketGuildUser>();
            Message = msg.Cast<SocketUserMessage>();
            GuildData = db.GetData(Guild);
        }

        public DiscordShardedClient Client { get; }
        public ServiceProvider ServiceProvider { get; }
        public SocketGuild Guild { get; }
        public SocketTextChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }

        public GuildData GuildData { get; }

        public Task ReactFailureAsync()
        {
            return Message.AddReactionAsync(new Emoji(_emojiService.X));
        }

        public Task ReactSuccessAsync()
        {
            return Message.AddReactionAsync(new Emoji(_emojiService.BallotBoxWithCheck));
        }

        public Embed CreateEmbed(string content)
        {
            return new EmbedBuilder().WithSuccessColor().WithAuthor(User)
                .WithDescription(content).Build();
        }

        public EmbedBuilder CreateEmbedBuilder(string content = null)
        {
            return new EmbedBuilder()
                .WithSuccessColor().WithAuthor(User).WithDescription(content ?? string.Empty);
        }

        public Task ReplyAsync(string content)
        {
            return Channel.SendMessageAsync(content);
        }

        public Task ReplyAsync(Embed embed)
        {
            return embed.SendToAsync(Channel);
        }

        public Task ReplyAsync(EmbedBuilder embed)
        {
            return embed.SendToAsync(Channel);
        }

        public Task ReactAsync(string unicode)
        {
            return Message.AddReactionAsync(new Emoji(unicode));
        }
    }
}