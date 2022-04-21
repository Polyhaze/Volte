using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactive;
using Volte.Services;

namespace Volte.Commands
{
    public class OkResult : ActionResult
    {
        public OkResult(string text, bool shouldEmbed = true, EmbedBuilder embed = null,
            Func<IUserMessage, Task> func = null, bool awaitCallback = true)
        {
            _message = text;
            _shouldEmbed = shouldEmbed;
            _messageCallback = func;
            _embed = embed;
            _runFuncAsync = awaitCallback;
        }

        public OkResult(IEnumerable<EmbedBuilder> pages, uint pageSplit = 0, Color? color = null,
            IGuildUser author = null,
            VolteContext ctx = null, string title = null, PaginatedAppearanceOptions options = null) : this(pages,
            pageSplit, color, author, ctx?.Message, title, options) { }

        public OkResult(IEnumerable<EmbedBuilder> pages, uint pageSplit = 0, Color? color = null, IGuildUser author = null,
            SocketUserMessage sourceMessage = null, string title = null, PaginatedAppearanceOptions options = null)
        {
            _pager = PaginatedMessage.NewBuilder()
                .WithPages(pages);

            if (color.HasValue)
                _pager.WithColor(color.Value);
            if (sourceMessage != null)
                _pager.WithDefaults(sourceMessage);
            if (title != null)
                _pager.WithTitle(title);
            if (options != null)
                _pager.WithOptions(options);
            if (pageSplit > 0)
                _pager.SplitPages(pageSplit);
            if (author != null)
                _pager.WithAuthor(author);
        }

        public OkResult(PaginatedMessage.Builder pager) => _pager = pager;

        public OkResult(Func<Task> logic, bool awaitFunc = true)
        {
            _separateLogic = logic;
            _runFuncAsync = awaitFunc;
        }

        public OkResult(PollInfo poll) => _poll = poll;

        private readonly bool _runFuncAsync;

        private readonly string _message;
        private readonly bool _shouldEmbed;
        private readonly PaginatedMessage.Builder _pager;
        private readonly Func<IUserMessage, Task> _messageCallback;
        private readonly Func<Task> _separateLogic;
        private readonly EmbedBuilder _embed;
        private readonly PollInfo _poll;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (!ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).SendMessages) return new ResultCompletionData();

            if (_poll != null)
                return new ResultCompletionData(await ctx.Interactive.StartPollAsync(ctx, _poll));

            if (_pager != null)
            {
                return new ResultCompletionData(
                    await ctx.Interactive.StartPagerAsync(ctx.Message, _pager.WithDefaults(ctx).Build()));
            }

            if (_separateLogic != null)
            {
                if (_runFuncAsync)
                    await _separateLogic();
                else
                    _ = _separateLogic();

                return new ResultCompletionData();
            }

            var data = ctx.Services.Get<DatabaseService>().GetData(ctx.Guild);

            var message = _embed is null
                ? _shouldEmbed
                    ? data.Configuration.ReplyInline
                        ? await ctx.CreateEmbed(_message).ReplyToAsync(ctx.Message)
                        : await ctx.CreateEmbed(_message).SendToAsync(ctx.Channel)
                    : data.Configuration.ReplyInline
                        ? await ctx.Message.ReplyAsync(_message, allowedMentions: AllowedMentions.None)
                        : await ctx.Channel.SendMessageAsync(_message, allowedMentions: AllowedMentions.None)
                : data.Configuration.ReplyInline
                    ? await _embed.ReplyToAsync(ctx.Message)
                    : await _embed.SendToAsync(ctx.Channel);


            /*IUserMessage message;
            if (_embed is null)
            {
                if (_shouldEmbed)
                    if (data.Configuration.ReplyInline)
                        message = await ctx.CreateEmbed(_message).ReplyToAsync(ctx.Message);
                    else
                        message = await ctx.CreateEmbed(_message).SendToAsync(ctx.Channel);
                else if (data.Configuration.ReplyInline)
                    message = await ctx.Message.ReplyAsync(_message);
                else
                    message = await ctx.Channel.SendMessageAsync(_message);
            }
            else if (ctx.GuildData.Configuration.ReplyInline)
                message = await _embed.ReplyToAsync(ctx.Message);
            else
                message = await _embed.SendToAsync(ctx.Channel);*/


            if (_messageCallback != null)
            {
                if (_runFuncAsync)
                    await _messageCallback(message);
                else
                    _ = _messageCallback(message);
            }


            return new ResultCompletionData(message);
        }
    }
}