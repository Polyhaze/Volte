using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Qmmands;
using Gommon;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands
{
    public sealed class VolteContext : CommandContext
    {
        public static VolteContext Create(DiscordMessage msg, IServiceProvider provider) 
            => new VolteContext(msg, provider);

        private VolteContext(DiscordMessage msg, IServiceProvider provider) : base(provider)
        {
            provider.Get<DatabaseService>(out var db);
            Client = provider.Get<DiscordShardedClient>();
            Guild = msg.Channel.Guild;
            Channel = msg.Channel;
            Member = msg.Author.Cast<DiscordMember>();
            Message = msg;
            GuildData = db.GetData(Guild);
            Now = DateTimeOffset.UtcNow;
            Interactivity = Client.GetInteractivity()[Client.GetShardId(Guild.Id)];
        }
        
        [NotNull]
        public readonly DiscordShardedClient Client;
        [NotNull]
        public readonly DiscordGuild Guild;
        [NotNull]
        public readonly DiscordChannel Channel;
        [NotNull]
        public readonly DiscordMember Member;
        [NotNull]
        public readonly DiscordMessage Message;
        [NotNull]
        public readonly InteractivityExtension Interactivity;
        [NotNull]
        public readonly GuildData GuildData;
        [NotNull]
        public readonly DateTimeOffset Now;

        public DiscordEmbed CreateEmbed(string content) => CreateEmbedBuilder(content).Build();

        public DiscordEmbedBuilder CreateEmbedBuilder(string content = null) => new DiscordEmbedBuilder()
            .WithColor(Member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor))
            .WithRequester(Member)
            .WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);

        public Task ReplyAsync(DiscordEmbed embed) => embed.SendToAsync(Channel);

        public Task ReplyAsync(DiscordEmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task ReactAsync(string unicode) => Message.CreateReactionAsync(unicode.ToEmoji());
    }
}