using Discord.Interactions;
using Volte.Systems.Interactions;
using RunMode = Qmmands.RunMode;

namespace Volte.Systems.Interactive;

public sealed class InteractiveService : VolteService, IDisposable
{
    private readonly DiscordSocketClient _client;
    
    private readonly InteractiveServiceConfig _config;

    private readonly Dictionary<ulong, IReactionCallback> _reactionCallbacks = [];
    private readonly Dictionary<ulong, IButtonCallback> _buttonCallbacks = [];

    public InteractiveService(DiscordSocketClient discord, InteractiveServiceConfig config = null)
    {
        _client = discord;
        _client.ReactionAdded += HandleReactionAsync;
        _client.InteractionCreated += interaction =>
            interaction is SocketMessageComponent { Data.Type: ComponentType.Button } component
                ? HandleComponentAsync(component)
                : Task.CompletedTask;

        _config = config ?? new InteractiveServiceConfig();
    }

    /// <summary>
    ///     Waits for the next message in the contextual channel.
    ///     This is a long-running <see cref="Task"/>.
    /// </summary>
    /// <param name="context">The context to wait on.</param>
    /// <param name="fromSourceUser">Should the message only be from the source user.</param>
    /// <param name="inSourceChannel">Should the message only be from the source channel.</param>
    /// <param name="timeout">The timeout to abort the waiting after.</param>
    /// <param name="token">The cancellation token to observe.</param>
    /// <returns>The waited message; or null if no message was received.</returns>
    public ValueTask<SocketUserMessage> NextMessageAsync(VolteContext context,
        bool fromSourceUser = true,
        bool inSourceChannel = true,
        TimeSpan? timeout = null,
        CancellationToken token = default)
    {
        var criterion = new Criteria<SocketUserMessage>();
        if (fromSourceUser)
            criterion.AddCriterion(new EnsureSourceUserCriterion());
        if (inSourceChannel)
            criterion.AddCriterion(new EnsureSourceChannelCriterion());
        return NextMessageAsync(context, criterion, timeout, token);
    }

    /// <summary>
    ///     Waits for the next message in the contextual channel.
    ///     This is a long-running <see cref="Task"/>.
    /// </summary>
    /// <param name="context">The context to wait on.</param>
    /// <param name="criterion">The <see cref="ICriterion{SocketUserMessage}"/> to use.</param>
    /// <param name="timeout">The timeout to abort the waiting after.</param>
    /// <param name="token">The cancellation token to observe.</param>
    /// <returns>The waited message; or null if no message was received.</returns>
    public async ValueTask<SocketUserMessage> NextMessageAsync(VolteContext context,
        ICriterion<SocketUserMessage> criterion,
        TimeSpan? timeout = null,
        CancellationToken token = default)
    {
        timeout ??= _config.DefaultTimeout;

        var msgTcs = new TaskCompletionSource<SocketUserMessage>();
        var cancelTcs = new TaskCompletionSource<bool>();

        token.Register(() => cancelTcs.SetResult(true));
        
        context.Client.MessageReceived += messageHandler;

        var trigger = msgTcs.Task;
        var task = await Task.WhenAny(trigger, Task.Delay(timeout.Value, token), cancelTcs.Task);

        context.Client.MessageReceived -= messageHandler;

        if (task == trigger)
            return await trigger;

        return null;
        
        async Task messageHandler(SocketMessage m)
        {
            Debug(LogSource.Service, m.Content);
            
            if (m.ShouldHandle(out var msg))
            {
                var result = await criterion.JudgeAsync(context, msg);
                if (result)
                    msgTcs.SetResult(msg);
            }
        }
    }

    /// <summary>
    ///     Sends a message to the contextual channel and deletes it after <paramref name="timeout"/> has ended.
    /// </summary>
    /// <param name="context">The context to use.</param>
    /// <param name="content">The content of the message to send. Can be empty if you're sending an embed.</param>
    /// <param name="isTts">Whether or not the message should use TTS. Defaults to false.</param>
    /// <param name="embed">The embed to send.</param>
    /// <param name="timeout">The time elapsed after the message is sent for it to be deleted.</param>
    /// <param name="options">The Discord.Net <see cref="RequestOptions"/> for the SendMessageAsync method.</param>
    /// <returns>The message that will be deleted.</returns>
    public async ValueTask<IUserMessage> ReplyAndDeleteAsync(VolteContext context,
        string content, bool isTts = false,
        Embed embed = null,
        TimeSpan? timeout = null,
        RequestOptions options = null)
    {
        timeout ??= _config.DefaultTimeout;
        var message = await context.Channel.SendMessageAsync(content, isTts, embed, options);
        _ = ExecuteAfterDelayAsync(timeout.Value, async () => await message.TryDeleteAsync());
        return message;
    }

    /// <summary>
    ///     Starts a poll in the contextual channel using the specified <see cref="PollInfo"/> applied to the embed.
    ///     This method does not start or in any way support reaction tracking.
    ///     This message will have its poll emojis added in the background so it's not a long-running <see cref="Task"/>.
    /// </summary>
    /// <param name="context">The context to use</param>
    /// <param name="pollInfo">The <see cref="PollInfo"/> to apply</param>
    /// <returns>The sent poll message.</returns>
    public static async ValueTask<IUserMessage> StartPollAsync(VolteContext context,
        PollInfo pollInfo)
    {
        var m = await context.CreateEmbedBuilder().Apply(pollInfo).SendToAsync(context.Channel);

        _ = Task.Run(async () =>
        {
            _ = await context.Message.TryDeleteAsync("Poll invocation message.");
            await DiscordHelper.GetPollEmojis().Take(pollInfo.Fields.Count)
                .ForEachAsync(emoji => m.AddReactionAsync(emoji));
        });
        return m;
    }

    public async ValueTask<IUserMessage> SendReactionPaginatedMessageAsync(VolteContext context,
        PaginatedMessage pager,
        ICriterion<SocketReaction> criterion = null)
    {
        var callback = new PaginatedMessageCallback(this, context, pager, criterion);
        await callback.DisplayAsync();
        return callback.Message;
    }
    
    public async ValueTask<IUserMessage> SendButtonPaginatedMessageAsync(SocketUserMessage sourceMessage,
        IServiceProvider provider,
        PaginatedMessage pager,
        ICriterion<SocketInteractionContext<SocketMessageComponent>> criterion = null)
    {
        var callback = new ButtonPaginatorCallback(this, sourceMessage, pager, provider,  criterion);
        await callback.StartAsync();
        return callback.PagerMessage;
    }
    
    public async ValueTask<IUserMessage> SendButtonPaginatedMessageAsync(VolteContext context,
        PaginatedMessage pager,
        ICriterion<SocketInteractionContext<SocketMessageComponent>> criterion = null)
    {
        var callback = new ButtonPaginatorCallback(this, context, pager, criterion);
        await callback.StartAsync();
        return callback.PagerMessage;
    }

    public void AddReactionCallback(IMessage message, IReactionCallback callback) =>
        _reactionCallbacks[message.Id] = callback;

    public bool RemoveReactionCallback(IMessage message) => RemoveReactionCallback(message.Id);
    public bool RemoveReactionCallback(ulong id) => _reactionCallbacks.Remove(id);
    public void ClearReactionCallbacks() => _reactionCallbacks.Clear();
    
    public void AddButtonCallback(IMessage message, IButtonCallback callback) =>
        _buttonCallbacks[message.Id] = callback;

    public bool RemoveButtonCallback(IMessage message) => RemoveReactionCallback(message.Id);
    public bool RemoveButtonCallback(ulong id) => _buttonCallbacks.Remove(id);
    public void ClearButtonCallbacks() => _buttonCallbacks.Clear();

    private async Task HandleComponentAsync(SocketMessageComponent component)
    {
        if (!_buttonCallbacks.TryGetValue(component.Message.Id, out var callback)) return;
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, component);
        var id = ctx.GetId();
        if (id.Identifier is not "pager") return;
        if (id.Value != callback.MessageContext.Message.Id.ToString()) return;
        if (!await callback.Criterion.JudgeAsync(callback.MessageContext, ctx)) return;

        var callbackTask = Task.Run(async () =>
        {
            if (await callback.HandleAsync(ctx) && RemoveButtonCallback(callback.PagerMessage.Id))
                Debug(LogSource.Service,
                    $"Button paginator for {callback.MessageContext.Message.Id} deleted, pager was ID {callback.PagerMessage.Id}");
        });

        if (callback.RunMode is RunMode.Sequential)
            await callbackTask;
    }
    

    private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> _,
        SocketReaction reaction)
    {
        if (reaction.UserId == _client.CurrentUser.Id) return;
        if (!_reactionCallbacks.TryGetValue(message.Id, out var callback)) return;
        if (!await callback.Criterion.JudgeAsync(callback.Context, reaction)) return;
        var callbackTask = Task.Run(async () =>
        {
            if (await callback.HandleAsync(reaction) && RemoveReactionCallback(await message.GetOrDownloadAsync()))
                Debug(LogSource.Service, $"Reaction paginator for {callback.Context.Message.Id} deleted");
        });

        if (callback.RunMode is RunMode.Sequential)
            await callbackTask;
    }

    public void Dispose() =>
        _client.ReactionAdded -= HandleReactionAsync;
}