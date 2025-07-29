using Discord.Interactions;
using Volte.Systems.Interactions;
using RunMode = Qmmands.RunMode;

namespace Volte.Systems.Interactive;

public class ButtonPaginatorCallback : IButtonCallback
{
    public VolteContext MessageContext { get; }
    public InteractiveService Interactive { get; }
    public IUserMessage PagerMessage { get; private set; }

    public RunMode RunMode { get; }
    public ICriterion<SocketInteractionContext<SocketMessageComponent>> Criterion { get; }

    private readonly PaginatedMessage _pager;

    private readonly int _pageCount;
    private int _currentPageIndex = 1;

    public ButtonPaginatorCallback(
        InteractiveService interactive,
        SocketUserMessage sourceMessage,
        PaginatedMessage pager,
        IServiceProvider provider,
        ICriterion<SocketInteractionContext<SocketMessageComponent>> criterion = null,
        RunMode runMode = RunMode.Sequential)
        : this(interactive, new VolteContext(sourceMessage, provider), pager, criterion, runMode)
    {
    }

    public ButtonPaginatorCallback(
        InteractiveService interactive,
        VolteContext sourceContext,
        PaginatedMessage pager,
        ICriterion<SocketInteractionContext<SocketMessageComponent>> criterion = null,
        RunMode runMode = RunMode.Sequential)
    {
        MessageContext = sourceContext;
        Interactive = interactive;
        Criterion = criterion ?? new EmptyCriterion<SocketInteractionContext<SocketMessageComponent>>();
        _pager = pager;

        if (_pager.Pages.First() is EmbedFieldBuilder)
            _pageCount = ((_pager.Pages.Count - 1) / _pager.Options.FieldsPerPage) + 1;
        else
            _pageCount = _pager.Pages.Count;

        RunMode = runMode;
    }

    public async Task StartAsync()
    {
        PagerMessage =
            await MessageContext.Channel.SendMessageAsync(_pager.Content,
                embed: BuildEmbed(),
                components: BuildComponent());
        Interactive.AddButtonCallback(PagerMessage, this);
    }

    public async ValueTask<bool> HandleAsync(SocketInteractionContext<SocketMessageComponent> button)
    {
        if (MessageContext.User.Id != button.User.Id)
        {
            await button.CreateReplyBuilder(true)
                .WithContent($"Only {MessageContext.User.Mention} may interact with this.")
                .RespondAsync();
            return false;
        }

        switch (button.GetId().Action)
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
                return await MessageContext.Message.TryDeleteAsync() && await PagerMessage.TryDeleteAsync();
            case "info":
                await button.CreateReplyBuilder(true).WithContent(_pager.Options.InformationText).RespondAsync();
                return false;
        }

        await button.Interaction.DeferAsync();
        await ReloadPagerMessageAsync();
        return false;
    }

    private MessageComponent BuildComponent() => new ComponentBuilder()
        .AddActionRow(x =>
            x.AddComponent(Buttons.Primary($"pager:back:{MessageContext.Message.Id}", label: "Back")
                    .WithEmote(_pager.Options.Back)
                    .WithDisabled(_currentPageIndex < 2))
                .AddComponent(Buttons.Primary($"pager:next:{MessageContext.Message.Id}", label: "Next")
                    .WithEmote(_pager.Options.Next)
                    .WithDisabled(_currentPageIndex >= _pageCount))
                .AddComponent(Buttons.Danger($"pager:stop:{MessageContext.Message.Id}", label: "End")
                    .WithEmote(_pager.Options.Stop))
        ).AddActionRow(x =>
            x.AddComponent(Buttons.Primary($"pager:first:{MessageContext.Message.Id}", label: "First")
                    .WithEmote(_pager.Options.First)
                    .WithDisabled(_currentPageIndex is 1))
                .AddComponent(Buttons.Primary($"pager:last:{MessageContext.Message.Id}", label: "Last")
                    .WithEmote(_pager.Options.Last)
                    .WithDisabled(_currentPageIndex == _pageCount))
                .AddComponentIf(_pager.Options.DisplayInformationIcon,
                    Buttons.Secondary($"pager:info:{MessageContext.Message.Id}", label: "Info")
                        .WithEmote(_pager.Options.Info))
        ).Build();

    private Embed BuildEmbed()
    {
        var currentElement = _pager.Pages.ElementAt(_currentPageIndex - 1);

        if (currentElement is EmbedBuilder embed)
        {
            if (!_pager.Title.IsNullOrWhitespace()) embed.WithTitle(_pager.Title);
            return embed.WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount)).Build();
        }

        var builder = MessageContext.CreateEmbedBuilder()
            .WithTitle(_pager.Title);

        if (_pager.Pages.Count > 1)
            builder.WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount));

        if (currentElement is EmbedFieldBuilder)
        {
            return builder.WithFields(_pager.Pages.OfType<EmbedFieldBuilder>()
                .Skip((_currentPageIndex - 1) * _pager.Options.FieldsPerPage)
                .Take(_pager.Options.FieldsPerPage).ToList()
            ).Build();
        }

        return builder.WithDescription(currentElement.ToString()).Build();
    }

    private Task ReloadPagerMessageAsync() => PagerMessage.ModifyAsync(m =>
    {
        m.Embed = BuildEmbed();
        m.Components = BuildComponent();
    });
}