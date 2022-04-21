using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Interactive;

namespace Volte.Services
{
    public class InteractiveService : IVolteService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordShardedClient _client;

        private readonly Dictionary<ulong, IReactionCallback> _reactionCallbacks;
        private readonly Dictionary<ulong, VolteButtonPaginator> _buttonPaginators;

        private readonly InteractiveServiceConfig _config;

        public InteractiveService(IServiceProvider provider, InteractiveServiceConfig config = null)
        {
            _provider = provider;
            _client = _provider.Get<DiscordShardedClient>();
            _client.ReactionAdded += HandleReactionAsync;
            _client.InteractionCreated += interaction =>
                interaction is SocketMessageComponent component
                    ? HandleComponentAsync(component)
                    : Task.CompletedTask;

            _config = config ?? new InteractiveServiceConfig();

            _reactionCallbacks = new Dictionary<ulong, IReactionCallback>();
            _buttonPaginators = new Dictionary<ulong, VolteButtonPaginator>();
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
            => NextMessageAsync(context, new Criteria<SocketUserMessage>()
                .AddCriterionIf(fromSourceUser, new EnsureSourceUserCriterion())
                .AddCriterionIf(inSourceChannel, new EnsureSourceChannelCriterion()), 
                timeout, token);


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

            async Task Handler(SocketMessage m)
            {
                if (m.ShouldHandle(out var msg))
                {
                    var result = await criterion.CheckAsync(context.Message, msg);
                    if (result)
                        msgTcs.SetResult(msg);
                }
            }

            context.Client.MessageReceived += Handler;

            var trigger = msgTcs.Task;
            var task = await Task.WhenAny(trigger, Task.Delay(timeout.Value, token), cancelTcs.Task);

            context.Client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger;

            return null;
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
            _ = Executor.ExecuteAfterDelayAsync(timeout.Value, async () => await message.TryDeleteAsync());
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
        public async ValueTask<IUserMessage> StartPollAsync(VolteContext context,
            PollInfo pollInfo)
        {
            var m = await context.CreateEmbedBuilder().Apply(pollInfo.Apply).SendToAsync(context.Channel);

            _ = Executor.ExecuteAsync(async () =>
            {
                await context.Message.TryDeleteAsync("Poll invocation message.");
                await DiscordHelper.GetPollEmojis()[..pollInfo.Fields.Count]
                    .ForEachAsync(emoji => m.AddReactionAsync(emoji));
            });
            return m;
        }

        public async ValueTask<IUserMessage> StartPagerAsync(SocketUserMessage message,
            PaginatedMessage pager, ICriterion<MessageComponentContext> criterion = null)
        {
            var callback = new VolteButtonPaginator(this, message, pager, criterion);
            await callback.StartAsync();
            return callback.PagerMessage;
        }


        public void AddReactionCallback(IMessage message, IReactionCallback callback) =>
            _reactionCallbacks[message.Id] = callback;

        public void AddButtonCallback(IMessage message, VolteButtonPaginator callback) =>
            _buttonPaginators[message.Id] = callback;

        public bool RemoveReactionCallback(IMessage message) => RemoveReactionCallback(message.Id);
        public bool RemoveReactionCallback(ulong id) => _reactionCallbacks.Remove(id);

        public bool RemoveButtonCallback(IMessage message) => RemoveButtonCallback(message.Id);
        public bool RemoveButtonCallback(ulong id) => _buttonPaginators.Remove(id);

        public void ClearReactionCallbacks() => _reactionCallbacks.Clear();
        public void ClearButtonCallbacks() => _buttonPaginators.Clear();

        private async Task HandleComponentAsync(SocketMessageComponent component)
        {
            if (!_buttonPaginators.TryGetValue(component.Message.Id, out var callback)) return;
            var ctx = new MessageComponentContext(component, _provider);
            if (ctx.Interaction.Data.Type != ComponentType.Button) return;
            if (ctx.Id.Identifier != "pager") return;
            if (ctx.Id.Value != callback.SourceMessage.Id.ToString()) return;
            if (!await callback.Criterion.CheckAsync(callback.SourceMessage, ctx)) return;

            var callbackTask = Executor.ExecuteAsync(async () =>
            {
                if (await callback.HandleAsync(ctx))
                    RemoveButtonCallback(callback.PagerMessage.Id);
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
            if (!await callback.Criterion.CheckAsync(callback.Context.Message, reaction)) return;
            var callbackTask = Executor.ExecuteAsync(async () =>
            {
                if (await callback.HandleAsync(reaction))
                    RemoveReactionCallback(message.Id);
            });

            if (callback.RunMode is RunMode.Sequential)
                await callbackTask;
        }

        public void Dispose() =>
            _client.ReactionAdded -= HandleReactionAsync;
    }
}