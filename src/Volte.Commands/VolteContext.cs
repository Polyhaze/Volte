using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Qmmands;
using Gommon;
using Humanizer;
using Volte.Core;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands
{
    public sealed class VolteContext : CommandContext
    {
        public static VolteContext Create(DiscordMessage msg, IServiceProvider provider) 
            => new VolteContext(msg, provider);

        // ReSharper disable once SuggestBaseTypeForParameter
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
        
        public readonly DiscordShardedClient Client;
        public readonly DiscordGuild Guild;
        public readonly DiscordChannel Channel;
        public readonly DiscordMember Member;
        public readonly DiscordMessage Message;
        public readonly InteractivityExtension Interactivity;
        public readonly GuildData GuildData;
        public readonly DateTimeOffset Now;

        public DiscordEmbed CreateEmbed(string content) => CreateEmbedBuilder(content).Build();

        public DiscordEmbedBuilder CreateEmbedBuilder(string content = null) => new DiscordEmbedBuilder()
            .WithColor(Member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor))
            .WithAuthor(Member.GetEffectiveUsername(), iconUrl: Member.AvatarUrl)
            .WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);

        public Task ReplyAsync(DiscordEmbed embed) => embed.SendToAsync(Channel);

        public Task ReplyAsync(DiscordEmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task ReactAsync(string unicode) => Message.CreateReactionAsync(unicode.ToEmoji());
    }
}