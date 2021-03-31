using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Gommon;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands
{
    public sealed class VolteContext : CommandContext
    {
        public static VolteContext Create(SocketMessage msg, IServiceProvider provider) 
            => new VolteContext(msg, provider);

        // ReSharper disable once SuggestBaseTypeForParameter
        private VolteContext(SocketMessage msg, IServiceProvider provider) : base(provider)
        {
            provider.Get<DatabaseService>(out var db);
            Client = provider.Get<DiscordShardedClient>();
            Guild = msg.Channel.Cast<SocketTextChannel>()?.Guild;
            Channel = msg.Channel.Cast<SocketTextChannel>();
            User = msg.Author.Cast<SocketGuildUser>();
            Message = msg.Cast<SocketUserMessage>();
            GuildData = db.GetData(Guild);
            Now = DateTime.Now;
        }
        
        

        public DiscordShardedClient Client { get; }
        public SocketGuild Guild { get; }
        public SocketTextChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }
        public GuildData GuildData { get; }
        public DateTime Now { get; }

        public Embed CreateEmbed(string content) => CreateEmbedBuilder(content).Build();

        public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
            .WithColor(User.GetHighestRoleWithColor()?.Color ?? new Color(Config.SuccessColor))
            .WithAuthor(User.ToString(), User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl())
            .WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);

        public Task ReplyAsync(Embed embed) => embed.SendToAsync(Channel);

        public Task ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}