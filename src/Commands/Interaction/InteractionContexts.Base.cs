using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Services;

namespace Volte.Commands.Interaction
{
    public abstract class InteractionContext<TBacking> where TBacking : SocketInteraction
    {
        public TBacking Backing { get; }
        public IServiceProvider Services { get; }
        public DatabaseService Db { get; }
        public DiscordShardedClient Client { get; }

        public SocketGuild Guild => Client.GetGuild(Backing.Channel.Cast<IGuildChannel>().GuildId);
        public bool InGuild => Guild != null;
        public SocketUser User => Backing.User;
        public SocketGuildUser GuildUser => Backing.User.Cast<SocketGuildUser>();
        public SocketTextChannel TextChannel => Channel.Cast<SocketTextChannel>();
        public SocketDMChannel DmChannel => Channel.Cast<SocketDMChannel>();
        public ISocketMessageChannel Channel => Backing.Channel;
        
        public InteractionReplyBuilder<TBacking> CreateReplyBuilder(bool ephemeral = false) 
            => new InteractionReplyBuilder<TBacking>(this).WithEphemeral(ephemeral);

        public Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
            => Backing.DeferAsync(ephemeral, options);

        public Task RespondAsync(
            string text = null,
            IEnumerable<Embed> embeds = null,
            bool isTts = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null)
            => Backing.RespondAsync(text, embeds?.ToArray(), isTts, ephemeral, allowedMentions, options, component);
        
        public Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            IEnumerable<Embed> embeds = null,
            bool isTts = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null)
            => Backing.FollowupAsync(text, embeds?.ToArray(), isTts, ephemeral, allowedMentions, options, component);

        public void ModifyGuildSettings(Action<GuildData> modifier)
            => Db.Save(GuildSettings.Apply(modifier));


        public EmbedBuilder CreateEmbedBuilder(string content = null)
            => new EmbedBuilder()
                .WithColor(GuildUser.GetHighestRole()?.Color ?? Config.SuccessColor)
                .WithDescription(content ?? string.Empty);

        public GuildData GuildSettings => Db.GetData(Guild?.Id);

        protected InteractionContext(TBacking backing, IServiceProvider provider)
        {
            Backing = backing;
            Services = provider;
            Client = provider.Get<DiscordShardedClient>();
            Db = provider.Get<DatabaseService>();
        }
    }
}