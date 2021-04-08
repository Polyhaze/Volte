using System;
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
        private readonly TimeSpan _defaultTimeout;

        // helpers to allow DI containers to resolve without a custom factory
        public InteractiveService(DiscordShardedClient discord, InteractiveServiceConfig config = null)
        {
            _client = discord;
            _client.ReactionAdded += HandleReactionAsync;

            config ??= new InteractiveServiceConfig();
            _defaultTimeout = config.DefaultTimeout;

            _callbacks = new Dictionary<ulong, IReactionCallback>();
        }

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

        public async Task<SocketUserMessage> NextMessageAsync(VolteContext context,
            ICriterion<SocketUserMessage> criterion,
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            timeout ??= _defaultTimeout;

            var msgTcs = new TaskCompletionSource<SocketUserMessage>();
            var cancelTcs = new TaskCompletionSource<bool>();

            token.Register(() => cancelTcs.SetResult(true));

            async Task Handler(SocketMessage message)
            {
                if (message.ShouldHandle(out var msg))
                {
                    var result = await criterion.JudgeAsync(context, msg);
                    if (result)
                        msgTcs.SetResult(msg);
                }
            }

            context.Client.MessageReceived += Handler;

            var trigger = msgTcs.Task;
            var task = await Task.WhenAny(trigger, Task.Delay(timeout.Value), cancelTcs.Task);

            context.Client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger;

            return null;
        }

        public async Task<IUserMessage> ReplyAndDeleteAsync(VolteContext context,
            string content, bool isTts = false,
            Embed embed = null,
            TimeSpan? timeout = null,
            RequestOptions options = null)
        {
            timeout ??= _defaultTimeout;
            var message = await context.Channel.SendMessageAsync(content, isTts, embed, options);
            _ = Executor.ExecuteAfterDelayAsync(timeout.Value, async () => await message.TryDeleteAsync());
            return message;
        }

        public async Task<IUserMessage> SendPaginatedMessageAsync(VolteContext context,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            var callback = new PaginatedMessageCallback(this, context, pager, criterion);
            await callback.DisplayAsync();
            return callback.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback)
            => _callbacks[message.Id] = callback;

        public void RemoveReactionCallback(IMessage message)
            => RemoveReactionCallback(message.Id);

        public void RemoveReactionCallback(ulong id)
            => _callbacks.Remove(id);

        public void ClearReactionCallbacks()
            => _callbacks.Clear();


        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id) return;
            if (!_callbacks.TryGetValue(message.Id, out var callback)) return;
            if (!await callback.Criterion.JudgeAsync(callback.Context, reaction)) return;
            var callbackTask = Executor.ExecuteAsync(async () =>
            {
                if (await callback.HandleAsync(reaction)) RemoveReactionCallback(message.Id);
            });
            
            if (callback.RunMode is RunMode.Sequential)
                await callbackTask;
        }

        public void Dispose()
        {
            _client.ReactionAdded -= HandleReactionAsync;
        }
    }
}