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
using Volte.Core.Helpers;
using Volte.Services;

namespace Volte.Interactive
{
    public class InteractiveService : VolteService, IDisposable
    {
        private readonly DiscordShardedClient _client;

        private readonly Dictionary<ulong, IReactionCallback> _callbacks;
        private readonly InteractiveServiceConfig _config;
        private readonly ConcurrentQueue<PaginatedMessageCallback> _activePagers;


        public InteractiveService(DiscordShardedClient discord, InteractiveServiceConfig config = null)
        {
            _client = discord;
            _client.ReactionAdded += HandleReactionAsync;

            _config = config ?? new InteractiveServiceConfig();

            _callbacks = new Dictionary<ulong, IReactionCallback>();
            _activePagers = new ConcurrentQueue<PaginatedMessageCallback>();
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
        public Task<SocketUserMessage> NextMessageAsync(VolteContext context,
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
        public async Task<SocketUserMessage> NextMessageAsync(VolteContext context,
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
                    var result = await criterion.JudgeAsync(context, msg);
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
        public async Task<IUserMessage> ReplyAndDeleteAsync(VolteContext context,
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

        public async Task<IUserMessage> SendPaginatedMessageAsync(VolteContext context,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            var callback = new PaginatedMessageCallback(this, context, pager, criterion);
            _activePagers.Enqueue(callback);
            await callback.DisplayAsync();
            return callback.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback) => _callbacks[message.Id] = callback;
        public bool RemoveReactionCallback(IMessage message) => RemoveReactionCallback(message.Id);
        public bool RemoveReactionCallback(ulong id) => _callbacks.Remove(id);
        public void ClearReactionCallbacks() => _callbacks.Clear();


        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id) return;
            if (!_callbacks.TryGetValue(message.Id, out var callback)) return;
            if (!await callback.Criterion.JudgeAsync(callback.Context, reaction)) return;
            var callbackTask = Executor.ExecuteAsync(async () =>
            {
                if (await callback.HandleAsync(reaction)) 
                    RemoveReactionCallback(await message.GetOrDownloadAsync());
            });

            if (callback.RunMode is RunMode.Sequential)
                await callbackTask;
        }

        public void Dispose() =>
            _client.ReactionAdded -= HandleReactionAsync;
    }
}