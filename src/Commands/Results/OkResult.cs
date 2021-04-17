using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qommon.Collections;
using Volte.Core.Helpers;
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
            _callback = func;
            _embed = embed;
            _runFuncAsync = awaitCallback;
        }

        public OkResult(IEnumerable<EmbedBuilder> pages, int pageSplit = -1, Color? color = null, IGuildUser author = null,
            VolteContext ctx = null, string title = null, PaginatedAppearanceOptions options = null)
        {
            _pager = PaginatedMessage.Builder.New
                .WithPages(pages);

            if (color.HasValue)
                _pager.WithColor(color.Value);
            if (author != null)
                _pager.WithAuthor(author);
            if (ctx != null)
                _pager.WithDefaults(ctx);
            if (title != null)
                _pager.WithTitle(title);
            if (options != null)
                _pager.WithOptions(options);
            if (pageSplit > 0)
                _pager.SplitPages(pageSplit);
        }

        public OkResult(PaginatedMessage.Builder pager) => _pager = pager;

        public OkResult(Func<Task> logic, bool awaitFunc = true)
        {
            _separateLogic = logic;
            _runFuncAsync = awaitFunc;
        }

        private readonly bool _runFuncAsync;

        private readonly string _message;
        private readonly bool _shouldEmbed;
        private readonly PaginatedMessage.Builder _pager;
        private readonly Func<IUserMessage, Task> _callback;
        private readonly Func<Task> _separateLogic;
        private readonly EmbedBuilder _embed;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (!ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).SendMessages) return new ResultCompletionData();

            if (_pager != null)
                return new ResultCompletionData(
                    await ctx.Interactive.SendPaginatedMessageAsync(ctx, _pager.WithDefaults(ctx).Build()));

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
                        ? await ctx.Message.ReplyAsync(_message)
                        : await ctx.Channel.SendMessageAsync(_message)
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


            if (_callback != null)
            {
                if (_runFuncAsync)
                    await _callback(message);
                else
                    _ = _callback(message);
            }


            return new ResultCompletionData(message);
        }
    }
}