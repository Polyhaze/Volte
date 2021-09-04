using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using Volte.Services;

namespace Volte.Interactions
{
    public class SlashCommandContext : InteractionContext<SocketSlashCommand>
    {
        public SlashCommandContext(SocketSlashCommand command, IServiceProvider provider) : base(command, provider) { }

        public SafeDictionary<string, ApplicationCommandOptionData> Options => Interaction.Data.GetOptions();
    }

    public class MessageCommandContext : InteractionContext<SocketMessageCommand>
    {
        public MessageCommandContext(SocketMessageCommand interaction, IServiceProvider provider) :
            base(interaction, provider) { }

        public SocketUserMessage UserMessage => Interaction.Data.Message.Cast<SocketUserMessage>();
    }

    public class UserCommandContext : InteractionContext<SocketUserCommand>
    {
        public UserCommandContext(SocketUserCommand interaction, IServiceProvider provider) : base(interaction, provider) { }

        public SocketGuildUser TargetedGuildUser => Interaction.Data.Member.Cast<SocketGuildUser>();
    }

    public class MessageComponentContext : InteractionContext<SocketMessageComponent>
    {
        public MessageComponentContext(SocketMessageComponent interaction, IServiceProvider provider) : base(interaction,
            provider) { }

        public string CustomId => Interaction.Data.CustomId;
        public string[] CustomIdParts => CustomId.Split(':');
        public SocketUserMessage Message => Interaction.Message;

        public IEnumerable<string> SelectedMenuOptions
            => Interaction.Data.Values?.ToHashSet() ?? new HashSet<string>();
    }

    public abstract class InteractionContext<TInteraction> where TInteraction : SocketInteraction
    {
        public TInteraction Interaction { get; }
        public IServiceProvider Services { get; }
        public DatabaseService Db { get; }
        public DiscordShardedClient Client { get; }

        public SocketGuild Guild => Client.GetGuild(Interaction.Channel.Cast<IGuildChannel>().GuildId);
        public bool InGuild => Guild != null;
        public SocketUser User => Interaction.User;
        public SocketGuildUser GuildUser => Interaction.User.Cast<SocketGuildUser>();
        public SocketTextChannel TextChannel => Channel.Cast<SocketTextChannel>();
        public SocketDMChannel DmChannel => Channel.Cast<SocketDMChannel>();
        public ISocketMessageChannel Channel => Interaction.Channel;

        public ReplyBuilder<TInteraction> CreateReplyBuilder(bool ephemeral = false)
            => new ReplyBuilder<TInteraction>(this).WithEphemeral(ephemeral);

        public Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
            => Interaction.DeferAsync(ephemeral, options);

        public Task UpdateAsync(Action<MessageProperties> func, RequestOptions options = null)
            => Interaction is SocketMessageComponent component
                ? component.UpdateAsync(func, options)
                : throw new InvalidOperationException("Cannot use UpdateAsync on an Interaction that isn't a message component.");
        

        public Task RespondAsync(
            string text = null,
            IEnumerable<Embed> embeds = null,
            bool isTts = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null)
            => Interaction.RespondAsync(text, embeds?.ToArray(), isTts, ephemeral, allowedMentions, options, component);

        public Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            IEnumerable<Embed> embeds = null,
            bool isTts = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null)
            => Interaction.FollowupAsync(text, embeds?.ToArray(), isTts, ephemeral, allowedMentions, options, component);

        public void ModifyGuildSettings(Action<GuildData> modifier)
            => Db.Save(GuildSettings.Apply(modifier));


        public EmbedBuilder CreateEmbedBuilder(string content = null)
            => new EmbedBuilder()
                .WithColor(GuildUser.GetHighestRole()?.Color ?? Config.SuccessColor)
                .WithDescription(content ?? string.Empty);

        public GuildData GuildSettings => Db.GetData(Guild?.Id);

        protected InteractionContext(TInteraction interaction, IServiceProvider provider)
        {
            Interaction = interaction;
            Services = provider;
            Client = provider.Get<DiscordShardedClient>();
            Db = provider.Get<DatabaseService>();
        }
    }
}