using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Gommon;
using Humanizer;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;
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
            Client = provider.Get<DiscordShardedClient>();
            Guild = msg.Channel.Cast<SocketTextChannel>()?.Guild;
            Interactive = provider.Get<InteractiveService>();
            Channel = msg.Channel.Cast<SocketTextChannel>();
            User = msg.Author.Cast<SocketGuildUser>();
            Message = msg.Cast<SocketUserMessage>();
            GuildData = provider.Get<DatabaseService>().GetData(Guild);
            Now = DateTime.Now;
        }


        public DiscordShardedClient Client { get; }
        public SocketGuild Guild { get; }
        public InteractiveService Interactive { get; }
        public SocketTextChannel Channel { get; }
        public SocketGuildUser User { get; }
        public SocketUserMessage Message { get; }
        public GuildData GuildData { get; }
        public DateTime Now { get; }
        
        public Embed CreateEmbed(StringBuilder content) => CreateEmbed(content.ToString());

        public Embed CreateEmbed(Action<EmbedBuilder> action) 
            => CreateEmbedBuilder().Apply(action).Build();
        
        public Embed CreateEmbed(string content) => CreateEmbedBuilder(content).Build();

        public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
            .WithColor(User.GetHighestRole()?.Color ?? new Color(Config.SuccessColor))
            .WithAuthor(User.ToString(), User.GetEffectiveAvatarUrl())
            .WithDescription(content ?? string.Empty);

        public EmbedBuilder CreateEmbedBuilder(StringBuilder content) => CreateEmbedBuilder(content.ToString());
        
        /// <summary>
        ///     Waits for a message containing content parseable by a registered <see cref="TypeParser{T}"/>.
        ///     Waiting times out after <paramref name="timeout"/> is over; returning no result and DidTimeout as true;
        ///     Receiving a message results in this method parsing its contents via the <see cref="TypeParser{T}"/>.
        ///     To disable a timeout, set it to <see cref="Timeout"/>.<see cref="Timeout.InfiniteTimeSpan"/>
        ///     TL;DR: Your <see cref="CommandService"/> must have a <see cref="TypeParser{T}"/> for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="timeout">The timespan to wait for. Defaults to 15 seconds.</param>
        /// <typeparam name="T">The type of object to wait for.</typeparam>
        public async ValueTask<(T Result, bool DidTimeout)> GetNextAsync<T>(TimeSpan? timeout = null)
        {
            timeout ??= 15.Seconds();
            var message = await Interactive.NextMessageAsync(this, timeout: timeout);
            if (message is null)
            {
                await CreateEmbed($"You didn't reply within {timeout.Value.Humanize()}. Run the command and try again.")
                    .SendToAsync(Channel);
                return (default, true);
            }

            var parserResult = await Services.Get<CommandService>().GetTypeParser<T>().ParseAsync(null, message.Content, this);
            if (parserResult.IsSuccessful)
                return (parserResult.Value, false);
            return (default, false);
        }

        public void Modify(Action<GuildData> modifier)
        {
            modifier(GuildData);
            if (Services.TryGet<DatabaseService>(out var db))
                db.Save(GuildData);
        }
    }
}