using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Interactive;
using Volte.Services;

namespace Volte.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public EventService EventService { get; set; }
        public ModLogService ModLogService { get; set; }
        public CommandService CommandService { get; set; }
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }
        public InteractiveService Interactive { get; set; }
        public new VolteContext Context => base.Context;

        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null, CancellationToken token = default(CancellationToken))
            => Interactive.NextMessageAsync(Context, criterion, timeout, token);
        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null, CancellationToken token = default(CancellationToken)) 
            => Interactive.NextMessageAsync(Context, fromSourceUser, inSourceChannel, timeout, token);

        public Task<IUserMessage> ReplyAndDeleteAsync(string content, bool isTts = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
            => Interactive.ReplyAndDeleteAsync(Context, content, isTts, embed, timeout, options);

        public async Task<IUserMessage> ReplyWithDeleteReactionAsync(string content = null, bool isTts = false, Embed embed = null,
            TimeSpan? timeout = null, RequestOptions options = null)
        {
            var m = await Context.Channel.SendMessageAsync(content ?? string.Empty, isTts, embed, options);
            await m.AddReactionAsync(EmojiService.X.ToEmoji());
            Interactive.AddReactionCallback(m, new DeleteMessageReactionCallback(Context));
            return m;
        }

        public Task<IUserMessage> PagedReplyAsync(List<object> pages, bool fromSourceUser = true)
        {
            var pager = new PaginatedMessage
            {
                Pages = pages
            };
            return PagedReplyAsync(pager, fromSourceUser);
        }
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            return PagedReplyAsync(pager, criterion);
        }
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ICriterion<SocketReaction> criterion)
            => Interactive.SendPaginatedMessageAsync(Context, pager, criterion);


        protected ActionResult Ok(
            string text, 
            Func<IUserMessage, Task> afterCompletion = null,
            bool shouldEmbed = true) 
            => new OkResult(text, shouldEmbed, null, afterCompletion);

        protected ActionResult Ok(
            Func<Task> logic, 
            bool awaitLogic = true) 
            => new OkResult(logic, awaitLogic);


        protected ActionResult Ok(
            EmbedBuilder embed, 
            Func<IUserMessage, Task> afterCompletion = null) 
            => new OkResult(null, true, embed, afterCompletion);

        protected ActionResult Ok(string text) 
            => new OkResult(text);

        protected ActionResult Ok(EmbedBuilder embed) 
            => new OkResult(null, true, embed);
        
        protected ActionResult Ok(PaginatedMessage message) 
            => new OkResult(message);

        protected ActionResult BadRequest(string reason) 
            => new BadRequestResult(reason);

        protected ActionResult None(
            Func<Task> afterCompletion = null, 
            bool awaitCallback = true) 
            => new NoResult(afterCompletion, awaitCallback);
    }
}