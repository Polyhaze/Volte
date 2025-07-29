namespace Volte.Systems.Interactive;

// ReSharper disable SuspiciousTypeConversion.Global
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
            _pageCount = ((_pager.Pages.Count - 1) / _pager.Options.FieldsPerPage) + 1;
        else
            _pageCount = _pager.Pages.Count;
    }

    public async Task DisplayAsync()
    {
        var embed = BuildEmbed();
        Message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed);
        if (_pager.Pages.Count > 1)
        {
            Interactive.AddReactionCallback(Message, this);
            // Reactions take a while to add, don't wait for them
            _ = Task.Run(async () =>
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
                    

                await Message.AddReactionAsync(_pager.Options.Stop);

                if (_pager.Options.DisplayInformationIcon)
                    await Message.AddReactionAsync(_pager.Options.Info);
            });
        }

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
            _ = Task.Run(async () =>
            {
                var response = await Interactive.NextMessageAsync(Context, 
                    new Criteria<SocketUserMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion(
                            (__, msg) => new ValueTask<bool>(msg.Content.TryParse<int>(out _))
                        ), 
                    15.Seconds());
                var req = response.Content.Parse<int>();

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
        else if (emote.Name.Equals("🛑"))
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
        var currentElement = _pager.Pages.ElementAt(_currentPageIndex - 1);

        if (currentElement is EmbedBuilder embed)
        {
            if (!_pager.Title.IsNullOrWhitespace()) embed.WithTitle(_pager.Title);
            return embed.WithFooter(_pager.Options.GenerateFooter(_currentPageIndex, _pageCount)).Build();
        }

        var builder = Context.CreateEmbedBuilder()
            .WithTitle(_pager.Title)
            .WithRelevantColor(Context.User);

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

    private Task RenderAsync() => Message.ModifyAsync(m => m.Embed = BuildEmbed());

    public async ValueTask DisposeAsync() => await Message.RemoveAllReactionsAsync();
        
}