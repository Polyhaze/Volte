using Volte.Systems.Database;
using Volte.Systems.Interactive;

namespace Volte.Systems.Commands.Text;

public class OkResult : ActionResult
{
    public OkResult(string text, bool shouldEmbed = true, EmbedBuilder embed = null,
        MessageCallback func = null, bool awaitCallback = true)
    {
        _message = text;
        _shouldEmbed = shouldEmbed;
        _messageCallback = func;
        _embed = embed;
        _runFuncAsync = awaitCallback;
    }

    public OkResult(IEnumerable<EmbedBuilder> pages, uint pageSplit = 0, Color? color = null, IGuildUser author = null,
        VolteContext ctx = null, string title = null, PaginatedAppearanceOptions options = null)
    {
        _pager = new PaginatedMessage.Builder()
            .WithPages(pages);

        if (color is { } clr) _pager.WithColor(clr);
        if (author is not null) _pager.WithAuthor(author);
        if (ctx is not null) _pager.WithDefaults(ctx);
        if (title is not null) _pager.WithTitle(title);
        if (options is not null) _pager.WithOptions(options);
        if (pageSplit > 0) _pager.SplitPages(pageSplit);
    }

    public OkResult(PaginatedMessage.Builder pager) => _pager = pager;

    public OkResult(AsyncFunction logic, bool awaitFunc = true)
    {
        _separateLogic = logic;
        _runFuncAsync = awaitFunc;
    }

    public OkResult(PollInfo poll) => _poll = poll;

    private readonly bool _runFuncAsync;

    private readonly string _message;
    private readonly bool _shouldEmbed;
    private readonly PaginatedMessage.Builder _pager;
    private readonly MessageCallback _messageCallback;
    private readonly AsyncFunction _separateLogic;
    private readonly EmbedBuilder _embed;
    private readonly PollInfo _poll;

    public override async ValueTask<Gommon.Optional<ResultCompletionData>> ExecuteResultAsync(VolteContext ctx)
    {
        if (!ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).SendMessages) return default;

        if (_poll != null)
            return new ResultCompletionData(await InteractiveService.StartPollAsync(ctx, _poll));
            
        if (_pager != null)
            return new ResultCompletionData(
                _pager.UseButtonPaginator
                    ? await ctx.Interactive.SendButtonPaginatedMessageAsync(ctx, _pager.WithDefaults(ctx).Build())
                    : await ctx.Interactive.SendReactionPaginatedMessageAsync(ctx, _pager.WithDefaults(ctx).Build())
                );

        if (_separateLogic != null)
        {
            if (_runFuncAsync)
                await _separateLogic();
            else
                _ = _separateLogic();

            return default;
        }

        var data = ctx.Services.Get<DatabaseService>().GetData(ctx.Guild);

        var message = _embed is null
            ? _shouldEmbed
                ? data.Settings.ReplyInline
                    ? await ctx.CreateEmbed(_message).ReplyToAsync(ctx.Message)
                    : await ctx.CreateEmbed(_message).SendToAsync(ctx.Channel)
                : data.Settings.ReplyInline
                    ? await ctx.Message.ReplyAsync(_message, allowedMentions: AllowedMentions.None)
                    : await ctx.Channel.SendMessageAsync(_message, allowedMentions: AllowedMentions.None)
            : data.Settings.ReplyInline
                ? await _embed.ReplyToAsync(ctx.Message)
                : await _embed.SendToAsync(ctx.Channel);

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