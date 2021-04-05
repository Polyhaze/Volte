using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Interactive
{
    public sealed class PaginatedMessageCallback : IReactionCallback
    {
        public VolteContext Context { get; }
        public InteractiveService Interactive { get; private set; }
        public IUserMessage Message { get; private set; }

        public RunMode RunMode => RunMode.Sequential;
        public ICriterion<SocketReaction> Criterion => _criterion;
        public TimeSpan? Timeout => options.Timeout;

        private readonly ICriterion<SocketReaction> _criterion;
        private readonly PaginatedMessage _pager;

        private PaginatedAppearanceOptions options => _pager.Options;
        private readonly int pages;
        private int page = 1;


        public PaginatedMessageCallback(InteractiveService interactive,
            VolteContext sourceContext,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            _criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            _pager = pager;
            if (_pager.Pages is IEnumerable<EmbedFieldBuilder>)
                pages = ((_pager.Pages.Count() - 1) / options.FieldsPerPage) + 1;
            else
                pages = _pager.Pages.Count();
        }

        public async Task DisplayAsync()
        {
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed).ConfigureAwait(false);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            // Reactions take a while to add, don't wait for them
            _ = Task.Run(async () =>
            {
                if (!(_pager.Pages.Count() is 1))
                {
                    await message.AddReactionAsync(options.First);
                    await message.AddReactionAsync(options.Back);
                    await message.AddReactionAsync(options.Next);
                    await message.AddReactionAsync(options.Last);
                    var manageMessages = (Context.Channel is IGuildChannel guildChannel) &&
                                         (Context.User as IGuildUser).GetPermissions(guildChannel).ManageMessages;

                    if (options.JumpDisplayOptions == JumpDisplayOptions.Always
                        || (options.JumpDisplayOptions == JumpDisplayOptions.WithManageMessages && manageMessages))
                        await message.AddReactionAsync(options.Jump);
                }

                await message.AddReactionAsync(options.Stop);

                if (options.DisplayInformationIcon)
                    await message.AddReactionAsync(options.Info);
            });

            if (Timeout != null)
            {
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Interactive.RemoveReactionCallback(message);
                    _ = Message.DeleteAsync();
                });
            }
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;

            if (emote.Equals(options.First))
                page = 1;
            else if (emote.Equals(options.Next))
            {
                if (page >= pages)
                    return false;
                ++page;
            }
            else if (emote.Equals(options.Back))
            {
                if (page <= 1)
                    return false;
                --page;
            }
            else if (emote.Equals(options.Last))
                page = pages;
            else if (emote.Equals(options.Stop))
            {
                await Message.TryDeleteAsync();
                return true;
            }
            else if (emote.Equals(options.Jump))
            {
                _ = Task.Run(async () =>
                {
                    var criteria = new Criteria<SocketMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion(new EnsureIsIntegerCriterion());
                    var response = await Interactive.NextMessageAsync(Context, criteria, TimeSpan.FromSeconds(15));
                    var req = int.Parse(response.Content);

                    if (req < 1 || req > pages)
                    {
                        _ = response.TryDeleteAsync();
                        await Interactive.ReplyAndDeleteAsync(Context, options.Stop.Name);
                        return;
                    }

                    page = req;
                    _ = response.TryDeleteAsync();
                    await RenderAsync();
                });
            }
            else if (emote.Equals(options.Info))
            {
                await Interactive.ReplyAndDeleteAsync(Context, options.InformationText, timeout: options.InfoTimeout);
                return false;
            }

            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync();
            return false;
        }

        private Embed BuildEmbed()
        {
            var initialEmbed = _pager.Pages.ElementAt(page - 1).Cast<EmbedBuilder>() ?? new EmbedBuilder();
            var builder = initialEmbed.WithAuthor(_pager.Author)
                .WithRelevantColor(Context.User)
                .WithFooter(string.Format(options.FooterFormat, page, pages))
                .WithTitle(_pager.Title ?? initialEmbed.Title);
            switch (_pager.Pages)
            {
                case IEnumerable<EmbedBuilder> _:
                    return builder.Build();
                case IEnumerable<EmbedFieldBuilder> efb:
                    builder.Fields = efb.Skip((page - 1) * options.FieldsPerPage).Take(options.FieldsPerPage).ToList();
                    builder.Description = _pager.AlternateDescription;
                    break;
                default:
                    builder.Description = _pager.Pages.ElementAt(page - 1).ToString();
                    break;
            }

            return builder.Build();
        }

        private async Task RenderAsync()
        {
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(false);
        }
    }
}