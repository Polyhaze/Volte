using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;

namespace Volte.Commands.Slash
{
    public class InteractionReplyBuilder<TInteraction> where TInteraction : SocketInteraction
    {
        private readonly InteractionContext<TInteraction> _context;

        public string Content { get; private set; }
        public HashSet<Embed> Embeds { get; } = new HashSet<Embed>();
        public bool IsTts { get; private set; }
        public bool IsEphemeral { get; private set; }
        public AllowedMentions AllowedMentions { get; private set; } = AllowedMentions.None;
        public MessageComponent Component { get; private set; }

        public InteractionReplyBuilder(InteractionContext<TInteraction> ctx) => _context = ctx;

        public InteractionReplyBuilder<TInteraction> WithContent(string content)
        {
            Content = content;
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithEmbed(Action<EmbedBuilder> modifier)
        {
            Embeds.Add(_context.CreateEmbedBuilder().Apply(modifier).Build());
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithEmbeds(params EmbedBuilder[] embedBuilders)
        {
            embedBuilders.ForEach(eb => Embeds.Add(eb.Build()));
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithEmbeds(params Embed[] embeds)
        {
            embeds.ForEach(e => Embeds.Add(e));
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithEmbedFrom(StringBuilder content) 
            => WithEmbedFrom(content.ToString());

        public InteractionReplyBuilder<TInteraction> WithEmbedFrom(string content)
        {
            Embeds.Add(_context.CreateEmbedBuilder(content).Build());
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithTts(bool tts)
        {
            IsTts = tts;
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithEphemeral(bool ephemeral = true)
        {
            IsEphemeral = ephemeral;
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithAllowedMentions(AllowedMentions allowedMentions)
        {
            AllowedMentions = allowedMentions;
            return this;
        }

        public InteractionReplyBuilder<TInteraction> WithComponent(ComponentBuilder builder)
        {
            Component = builder.Build();
            return this;
        }
        
        public InteractionReplyBuilder<TInteraction> WithActionRows(params ActionRowBuilder[] actionRows)
        {
            Component = new ComponentBuilder()
                .AddActionRows(actionRows)
                .Build();
            return this;
        }
        
        public InteractionReplyBuilder<TInteraction> WithActionRows(IEnumerable<ActionRowBuilder> actionRows)
            => WithActionRows(actionRows.ToArray());

        public InteractionReplyBuilder<TInteraction> WithButtons(IEnumerable<ButtonBuilder> buttons) 
            => WithActionRows(buttons.Select(x => x.Build()).AsActionRow());


        public InteractionReplyBuilder<TInteraction> WithSelectMenu(SelectMenuBuilder menu)
        {
            Component = new ComponentBuilder().WithSelectMenu(menu).Build();
            return this;
        }

        public Task RespondAsync(RequestOptions options = null)
            => _context.RespondAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, options, Component);

        public Task<RestFollowupMessage> FollowupAsync(RequestOptions options = null)
            => _context.FollowupAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, options, Component);
    }
}