using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Services;

namespace Volte.Interactive
{
    public class ButtonPaginatorCallback : IButtonCallback
    {
        public SocketUserMessage SourceMessage { get; }
        public InteractiveService Interactive { get; }
        public IUserMessage PagerMessage { get; private set; }

        public RunMode RunMode => RunMode.Sequential;
        public ICriterion<MessageComponentContext> Criterion { get; }

        private readonly PaginatedMessage _pager;

        private readonly int _pageCount;
        private int _currentPageIndex = 1;


        public ButtonPaginatorCallback(InteractiveService interactive,
            SocketUserMessage sourceMessage,
            PaginatedMessage pager,
            ICriterion<MessageComponentContext> criterion = null)
        {
            SourceMessage = sourceMessage;
            Interactive = interactive;
            Criterion = criterion ?? new EmptyCriterion<MessageComponentContext>();
            _pager = pager;
            if (_pager.Pages is IEnumerable<EmbedFieldBuilder>)
                _pageCount = ((_pager.Pages.Count() - 1) / _pager.Options.FieldsPerPage) + 1;
            else
                _pageCount = _pager.Pages.Count();
        }

        private MessageComponent BuildComponent() => new ComponentBuilder()
            .AddActionRow(x =>
                x.AddComponent(new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:back")
                        .WithLabel("Back")
                        .WithEmote(_pager.Options.Back)
                        .WithDisabled(_currentPageIndex < 2)
                        .WithStyle(ButtonStyle.Primary)
                        .Build())
                    .AddComponent(new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:next")
                        .WithLabel("Next")
                        .WithEmote(_pager.Options.Next)
                        .WithDisabled(_currentPageIndex >= _pageCount)
                        .WithStyle(ButtonStyle.Primary)
                        .Build())
                    .AddComponent(new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:stop")
                        .WithLabel("End")
                        .WithEmote(_pager.Options.Stop)
                        .WithStyle(ButtonStyle.Danger)
                        .Build())
            ).AddActionRow(x => 
                x.AddComponent(new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:first")
                        .WithLabel("First")
                        .WithEmote(_pager.Options.First)
                        .WithDisabled(_currentPageIndex is 1)
                        .WithStyle(ButtonStyle.Primary)
                        .Build())
                    .AddComponent(new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:last")
                        .WithLabel("Last")
                        .WithEmote(_pager.Options.Last)
                        .WithDisabled(_currentPageIndex == _pageCount)
                        .WithStyle(ButtonStyle.Primary)
                        .Build())
                    .AddComponentIf(_pager.Options.DisplayInformationIcon, new ButtonBuilder()
                        .WithCustomId($"pager:{SourceMessage.Id}:info")
                        .WithLabel("Info")
                        .WithEmote(_pager.Options.Info)
                        .WithStyle(ButtonStyle.Secondary)
                        .Build())
            ).Build();

        public async Task StartAsync()
        {
            PagerMessage = await SourceMessage.Channel.SendMessageAsync(_pager.Content, embed: BuildEmbed(), component: BuildComponent());
            Interactive.AddButtonCallback(PagerMessage, this);
        }

        public async ValueTask<bool> HandleAsync(MessageComponentContext button)
        {
            if (SourceMessage.Author.Id != button.User.Id)
            {
                await button.CreateReplyBuilder(true)
                    .WithContent($"Only {SourceMessage.Author.Mention} may interact with this.")
                    .RespondAsync();
                return false;
            }
            
            switch (button.CustomIdParts[2])
            {
                case "first":
                    _currentPageIndex = 1;
                    break;
                case "next":
                    if (_currentPageIndex >= _pageCount)
                        return false;
                    _currentPageIndex++;
                    break;
                case "back":
                    if (_currentPageIndex <= 1)
                        return false;
                    _currentPageIndex--;
                    break;
                case "last":
                    _currentPageIndex = _pageCount;
                    break;
                case "stop":
                    return await SourceMessage.TryDeleteAsync() && await PagerMessage.TryDeleteAsync();
                case "info":
                    await button.CreateReplyBuilder(true).WithContent(_pager.Options.InformationText).RespondAsync();
                    return false;
            }

            await button.DeferAsync();
            await ReloadPagerMessageAsync();
            return false;
        }

        private Embed BuildEmbed()
        {
            if (_pager.Pages is IEnumerable<EmbedBuilder> embeds)
            {
                var e = embeds.ElementAt(_currentPageIndex - 1);
                if (!_pager.Title.IsNullOrWhitespace()) e.WithTitle(_pager.Title);
                return e.WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount)).Build();
            }

            var member = SourceMessage.Author.Cast<SocketGuildUser>();

            var builder = new EmbedBuilder()
                .WithColor(_pager.Color)
                .WithAuthor(member.ToString(), member.GetEffectiveAvatarUrl())
                .WithTitle(_pager.Title)
                .WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount));

            return (_pager.Pages switch
            {
                IEnumerable<EmbedFieldBuilder> efb => builder.WithFields(efb
                    .Skip((_currentPageIndex - 1) * _pager.Options.FieldsPerPage)
                    .Take(_pager.Options.FieldsPerPage).ToList()),
                _ => builder.WithDescription(_pager.Pages.ElementAt(_currentPageIndex - 1).ToString())
            }).Build();
        }

        private Task ReloadPagerMessageAsync() => PagerMessage.ModifyAsync(m =>
        {
            m.Embed = BuildEmbed();
            m.Components = BuildComponent();
        });
    }
}