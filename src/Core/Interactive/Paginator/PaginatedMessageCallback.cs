using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;

namespace Volte.Interactive
{
    public sealed class PaginatedMessageCallback : IReactionCallback
    {
        public VolteContext Context { get; }
        public InteractiveService Interactive { get; }
        public IUserMessage Message { get; private set; }

        public RunMode RunMode => RunMode.Sequential;
        public ICriterion<SocketReaction> Criterion { get; }

        public TimeSpan? Timeout => _pager.Options.Timeout;

        private readonly PaginatedMessage _pager;
        
        private readonly int _pageCount;
        private int _currentPageIndex = 1;


        public PaginatedMessageCallback(InteractiveService interactive,
            VolteContext sourceContext,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            Criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            _pager = pager;
            if (_pager.Pages is IEnumerable<EmbedFieldBuilder>)
                _pageCount = ((_pager.Pages.Count() - 1) / _pager.Options.FieldsPerPage) + 1;
            else
                _pageCount = _pager.Pages.Count();
        }

        public async Task DisplayAsync()
        {
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed).ConfigureAwait(false);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            // Reactions take a while to add, don't wait for them
            _ = Executor.ExecuteAsync(async () =>
            {
                if (!(_pager.Pages.Count() is 1))
                {
                    await message.AddReactionAsync(_pager.Options.First);
                    await message.AddReactionAsync(_pager.Options.Back);
                    await message.AddReactionAsync(_pager.Options.Next);
                    await message.AddReactionAsync(_pager.Options.Last);
                    var manageMessages = Context.Channel is IGuildChannel guildChannel &&
                                         (Context.User as IGuildUser).GetPermissions(guildChannel).ManageMessages;

                    if (_pager.Options.JumpDisplayOptions == JumpDisplayOptions.Always
                        || (_pager.Options.JumpDisplayOptions == JumpDisplayOptions.WithManageMessages && manageMessages))
                        await message.AddReactionAsync(_pager.Options.Jump);
                }

                await message.AddReactionAsync(_pager.Options.Stop);

                if (_pager.Options.DisplayInformationIcon)
                    await message.AddReactionAsync(_pager.Options.Info);
            });

            if (Timeout != null)
            {
                Executor.ExecuteAfterDelay(Timeout.Value, () =>
                {
                    Interactive.RemoveReactionCallback(message);
                    _ = Message.DeleteAsync();
                });
            }
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;

            if (emote.Equals(_pager.Options.First))
                _currentPageIndex = 1;
            else if (emote.Equals(_pager.Options.Next))
            {
                if (_currentPageIndex >= _pageCount)
                    return false;
                _currentPageIndex++;
            }
            else if (emote.Equals(_pager.Options.Back))
            {
                if (_currentPageIndex <= 1)
                    return false;
                _currentPageIndex--;
            }
            else if (emote.Equals(_pager.Options.Last))
                _currentPageIndex = _pageCount;
            else if (emote.Equals(_pager.Options.Stop))
            {
                await Message.TryDeleteAsync();
                return true;
            }
            else if (emote.Equals(_pager.Options.Jump))
            {
                _ = Executor.ExecuteAsync(async () =>
                {
                    var criteria = new Criteria<SocketMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion(new EnsureIsIntegerCriterion());
                    var response = await Interactive.NextMessageAsync(Context, criteria, 15.Seconds());
                    var req = int.Parse(response.Content);

                    if (req < 1 || req > _pageCount)
                    {
                        _ = response.TryDeleteAsync();
                        await Interactive.ReplyAndDeleteAsync(Context, _pager.Options.Stop.Name, timeout: 3.Seconds());
                        return;
                    }

                    _currentPageIndex = req;
                    _ = response.TryDeleteAsync();
                    await RenderAsync();
                });
            }
            else if (emote.Equals(_pager.Options.Info))
            {
                await Interactive.ReplyAndDeleteAsync(Context, _pager.Options.InformationText, timeout: _pager.Options.InfoTimeout);
                return false;
            }

            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync();
            return false;
        }

        private Embed BuildEmbed()
        {
            if (_pager.Pages is IEnumerable<EmbedBuilder> embeds)
            {
                var e = embeds.ElementAt(_currentPageIndex - 1);
                if (!_pager.Title.IsNullOrWhitespace()) e.WithTitle(_pager.Title);
                return e.WithFooter(string.Format(_pager.Options.FooterFormat, _currentPageIndex, _pageCount)).Build();
            }
            
            var builder = Context.CreateEmbedBuilder()
                .WithTitle(_pager.Title)
                .WithRelevantColor(Context.User)
                .WithFooter(string.Format(_pager.Options.FooterFormat, _currentPageIndex, _pageCount));
            switch (_pager.Pages)
            {
                case IEnumerable<EmbedFieldBuilder> efb:
                    builder.Fields = efb.Skip((_currentPageIndex - 1) * _pager.Options.FieldsPerPage).Take(_pager.Options.FieldsPerPage).ToList();
                    builder.Description = _pager.AlternateDescription;
                    break;
                default:
                    builder.Description = _pager.Pages.ElementAt(_currentPageIndex - 1).ToString();
                    break;
            }

            return builder.Build();
        }

        private Task RenderAsync() => Message.ModifyAsync(m => m.Embed = BuildEmbed());

    }
}