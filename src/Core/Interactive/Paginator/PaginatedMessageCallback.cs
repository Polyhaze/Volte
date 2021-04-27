using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Core.Helpers;
using Volte.Services;

namespace Volte.Interactive
{
    public sealed class PaginatedMessageCallback : IReactionCallback, IAsyncDisposable
    {
        public VolteContext Context { get; }
        public InteractiveService Interactive { get; }
        public IUserMessage Message { get; private set; }

        public RunMode RunMode => RunMode.Sequential;
        public ICriterion<SocketReaction> Criterion { get; }

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
            Message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed);
            Interactive.AddReactionCallback(Message, this);
            // Reactions take a while to add, don't wait for them
            _ = Executor.ExecuteAsync(async () =>
            {
                if (!_pager.Pages.IsEmpty())
                {
                    await Message.AddReactionAsync(_pager.Options.First);
                    await Message.AddReactionAsync(_pager.Options.Back);
                    await Message.AddReactionAsync(_pager.Options.Next);
                    await Message.AddReactionAsync(_pager.Options.Last);
                    var manageMessages = Context.Channel is IGuildChannel guildChannel &&
                                         (Context.User as IGuildUser).GetPermissions(guildChannel).ManageMessages;

                    if (_pager.Options.JumpDisplayOptions == JumpDisplayOptions.Always
                        || (_pager.Options.JumpDisplayOptions == JumpDisplayOptions.WithManageMessages && manageMessages))
                        await Message.AddReactionAsync(_pager.Options.Jump);
                }

                await Message.AddReactionAsync(_pager.Options.Stop);

                if (_pager.Options.DisplayInformationIcon)
                    await Message.AddReactionAsync(_pager.Options.Info);
            });
        }

        public async ValueTask<bool> HandleAsync(SocketReaction reaction)
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
                return await Context.Message.TryDeleteAsync() && await Message.TryDeleteAsync();
            else if (emote.Equals(_pager.Options.Jump))
            {
                _ = Executor.ExecuteAsync(async () =>
                {
                    var response = await Interactive.NextMessageAsync(Context, new Criteria<SocketUserMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion((__, msg) => new ValueTask<bool>(int.TryParse(msg.Content, out _))), 15.Seconds());
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
            else if (emote.Name.Equals(DiscordHelper.OctagonalSign))
            {
                await DisposeAsync();
                return true;
            }

            await Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync();
            return false;
        }

        private Embed BuildEmbed()
        {
            if (_pager.Pages is IEnumerable<EmbedBuilder> embeds)
            {
                var e = embeds.ElementAt(_currentPageIndex - 1);
                if (!_pager.Title.IsNullOrWhitespace()) e.WithTitle(_pager.Title);
                var footer = _pager.Options.GenerateFooter(_currentPageIndex, _pageCount);
                return e.WithFooter(e.Footer is null ? footer : $"{footer} | {e.Footer.Text}").Build();
            }
            
            var builder = Context.CreateEmbedBuilder()
                .WithTitle(_pager.Title)
                .WithRelevantColor(Context.User)
                .WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount));

            return (_pager.Pages switch
            {
                IEnumerable<EmbedFieldBuilder> efb => builder.WithFields(efb
                    .Skip((_currentPageIndex - 1) * _pager.Options.FieldsPerPage)
                    .Take(_pager.Options.FieldsPerPage).ToList()),
                _ => builder.WithDescription(_pager.Pages.ElementAt(_currentPageIndex - 1).ToString())
            }).Build();
        }

        private Task RenderAsync() => Message.ModifyAsync(m => m.Embed = BuildEmbed());

        public async ValueTask DisposeAsync()
        {
            await Message.RemoveAllReactionsAsync();
        }
    }
}