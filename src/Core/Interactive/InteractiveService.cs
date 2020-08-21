using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Qmmands;
using Volte.Commands;
using Volte.Services;

namespace Volte.Interactive
{
    public class InteractiveService : VolteService, IDisposable
    {
        private readonly DiscordShardedClient _client;

        private readonly Dictionary<ulong, IReactionCallback> _callbacks;
        private readonly TimeSpan _defaultTimeout;

        // helpers to allow DI containers to resolve without a custom factory
        public InteractiveService(DiscordShardedClient discord, TimeSpan? defaultTimeout = null)
        {
            _client = discord;
            _client.ReactionAdded += HandleReactionAsync;

            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(15);

            _callbacks = new Dictionary<ulong, IReactionCallback>();
        }

        public Task<SocketMessage> NextMessageAsync(VolteContext context, 
            bool fromSourceUser = true, 
            bool inSourceChannel = true, 
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            return NextMessageAsync(context, new Criteria<SocketMessage>()
                .AddCriterionIf(fromSourceUser, new EnsureSourceUserCriterion())
                .AddCriterionIf(inSourceChannel, new EnsureSourceChannelCriterion()), timeout, token);
        }
        
        public async Task<SocketMessage> NextMessageAsync(VolteContext context, 
            ICriterion<SocketMessage> criterion, 
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            timeout ??= _defaultTimeout;

            var eventTrigger = new TaskCompletionSource<SocketMessage>();
            var cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true));

            async Task Handler(SocketMessage message)
            {
                var result = await criterion.JudgeAsync(context, message);
                if (result)
                    eventTrigger.SetResult(message);
            }

            context.Client.MessageReceived += Handler;

            var trigger = eventTrigger.Task;
            var cancel = cancelTrigger.Task;
            var delay = Task.Delay(timeout.Value);
            var task = await Task.WhenAny(trigger, delay, cancel);

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
            _ = Task.Delay(timeout.Value)
                .ContinueWith(_ => message.DeleteAsync());
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
            if (!await callback.Criterion.JudgeAsync(callback.Context, reaction))
                return;
            switch (callback.RunMode)
            {
                case RunMode.Parallel:
                    _ = Task.Run(async () =>
                    {
                        if (await callback.HandleCallbackAsync(reaction))
                            RemoveReactionCallback(message.Id);
                    });
                    break;
                case RunMode.Sequential:
                    if (await callback.HandleCallbackAsync(reaction))
                        RemoveReactionCallback(message.Id);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot perform an Interactivity callback with an invalid RunMode. Received: {callback.RunMode}.");
            }
        }

        public void Dispose()
        {
            _client.ReactionAdded -= HandleReactionAsync;
        }
    }
}
