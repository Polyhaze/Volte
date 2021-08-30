using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;

namespace Volte.Interactions
{
    public class ReplyBuilder<TInteraction> where TInteraction : SocketInteraction
    {
        private readonly InteractionContext<TInteraction> _context;

        public string Content { get; private set; }
        public HashSet<Embed> Embeds { get; } = new HashSet<Embed>();
        public bool IsTts { get; private set; }
        public bool IsEphemeral { get; private set; }
        public AllowedMentions AllowedMentions { get; private set; } = AllowedMentions.None;
        public Task UpdateOrNoopTask => _updateTask ?? Task.CompletedTask;
        private Task _updateTask;
        public HashSet<ActionRowBuilder> ActionRows { get; } = new HashSet<ActionRowBuilder>();

        public ReplyBuilder(InteractionContext<TInteraction> ctx) => _context = ctx;

        public ReplyBuilder<TInteraction> WithContent(string content)
        {
            Content = content;
            return this;
        }

        public ReplyBuilder<TInteraction> WithEmbed(Action<EmbedBuilder> modifier)
        {
            Embeds.Add(_context.CreateEmbedBuilder().Apply(modifier).Build());
            return this;
        }

        public ReplyBuilder<TInteraction> WithEmbeds(IEnumerable<EmbedBuilder> embedBuilders)
        {
            embedBuilders.ForEach(x => Embeds.Add(x.Build()));
            return this;
        }
        
        public ReplyBuilder<TInteraction> WithEmbeds(IEnumerable<Embed> embeds)
        {
            embeds.ForEach(x => Embeds.Add(x));
            return this;
        }

        public ReplyBuilder<TInteraction> WithEmbeds(params EmbedBuilder[] embedBuilders) =>
            WithEmbeds(embedBuilders.Select(x => x.Build()));

        public ReplyBuilder<TInteraction> WithEmbeds(params Embed[] embeds) => WithEmbeds(embeds.ToList());

        public ReplyBuilder<TInteraction> WithEmbedFrom(StringBuilder content) 
            => WithEmbedFrom(content.ToString());

        public ReplyBuilder<TInteraction> WithEmbedFrom(string content)
        {
            Embeds.Add(_context.CreateEmbedBuilder(content).Build());
            return this;
        }

        public ReplyBuilder<TInteraction> WithTts(bool tts)
        {
            IsTts = tts;
            return this;
        }

        public ReplyBuilder<TInteraction> WithEphemeral(bool ephemeral = true)
        {
            IsEphemeral = ephemeral;
            return this;
        }

        public ReplyBuilder<TInteraction> WithAllowedMentions(AllowedMentions allowedMentions)
        {
            AllowedMentions = allowedMentions;
            return this;
        }

        public ReplyBuilder<TInteraction> WithComponentMessageUpdate(Action<MessageProperties> modifier)
        {
            if (_context is MessageComponentContext mctx)
                _updateTask = mctx.Backing.Message.ModifyAsync(modifier);
            
            return this;
        }

        public ReplyBuilder<TInteraction> WithComponent(ComponentBuilder builder)
        {
            WithActionRows(builder.ActionRows);
            return this;
        }
        
        public ReplyBuilder<TInteraction> WithActionRows(params ActionRowBuilder[] actionRows)
        {
            actionRows.ForEach(row => ActionRows.Add(row));
            return this;
        }
        
        public ReplyBuilder<TInteraction> WithActionRows(IEnumerable<ActionRowBuilder> actionRows)
            => WithActionRows(actionRows.ToArray());

        public ReplyBuilder<TInteraction> WithButtons(IEnumerable<ButtonBuilder> buttons) 
            => WithActionRows(buttons.Select(x => x.Build()).AsActionRow());
        
        public ReplyBuilder<TInteraction> WithButtons(params ButtonBuilder[] buttons) 
            => WithActionRows(buttons.Select(x => x.Build()).AsActionRow());


        public ReplyBuilder<TInteraction> WithSelectMenu(SelectMenuBuilder menu)
        {
            ActionRows.Add(new ActionRowBuilder().AddComponent(menu.Build()));
            return this;
        }

        public async Task RespondAsync(RequestOptions options = null)
        {
            await _context.RespondAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, options, new ComponentBuilder().AddActionRows(ActionRows).Build());
            await UpdateOrNoopTask;
        }

        public async Task<RestFollowupMessage> FollowupAsync(RequestOptions options = null)
        {
            var result = await _context.FollowupAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, options, new ComponentBuilder().AddActionRows(ActionRows).Build());
            await UpdateOrNoopTask;
            return result;
        }
    }
}