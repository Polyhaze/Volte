namespace Volte.Services;

public sealed class MessageService : VolteService
{
    private readonly DatabaseService _db;
    private readonly CommandService _commandService;
    private readonly QuoteService _quoteService;
        
    public MessageService(DatabaseService databaseService,
        CommandService commandService,
        QuoteService quoteService)
    {
        _db = databaseService;
        _commandService = commandService;
        _quoteService = quoteService;
    }
    
    public ulong AllTimeCommandCalls => CalledCommandsInfo.Sum + 
                                        UnsavedSuccessfulCommandCalls + 
                                        UnsavedFailedCommandCalls;
    public ulong AllTimeSuccessfulCommandCalls => CalledCommandsInfo.Successes + UnsavedSuccessfulCommandCalls;
    public ulong AllTimeFailedCommandCalls => CalledCommandsInfo.Failures + UnsavedFailedCommandCalls;
    
    public ulong UnsavedSuccessfulCommandCalls { get; private set; }
    public ulong UnsavedFailedCommandCalls { get; private set; }

    public void ResetCalledCommands()
    {
        UnsavedSuccessfulCommandCalls = 0;
        UnsavedFailedCommandCalls = 0;
    }

    public async Task HandleMessageAsync(MessageReceivedEventArgs args)
    {
        List<string> prefixes = [
            args.Data.Configuration.CommandPrefix, 
            $"<@{args.Context.Client.CurrentUser.Id}> ",
            $"<@!{args.Context.Client.CurrentUser.Id}> "
        ];

        if (CommandUtilities.HasAnyPrefix(args.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
                out var cmd))
        {
            var sw = Stopwatch.StartNew();
            var result = await _commandService.ExecuteAsync(cmd, args.Context);
            sw.Stop();
            if (result is not CommandNotFoundResult)
                await OnCommandAsync(new CommandCalledEventArgs(result, args.Context, sw));
        }
        else
        {
            if (args.Message.Content.EqualsAnyIgnoreCase($"<@{args.Context.Client.CurrentUser.Id}>",
                    $"<@!{args.Context.Client.CurrentUser.Id}>"))
            {
                await args.Context.CreateEmbed(
                        $"The prefix for this guild is **{args.Data.Configuration.CommandPrefix}**; " +
                        $"alternatively you can just mention me as a prefix, i.e. `@{args.Context.Guild.CurrentUser} help`.")
                    .ReplyToAsync(args.Message);
            }
            else if (!await _quoteService.CheckMessageAsync(args))
                if (CommandUtilities.HasPrefix(args.Message.Content, '%', out var tagName))
                {
                    if (!args.Data.Extras.GetTagByNameOrAlias(tagName).TryGet(out var tag))
                        return;

                    tag.Uses++;
                    _db.Save(args.Data);

                    if (args.Data.Configuration.EmbedTagsAndShowAuthor)
                        await tag.AsEmbed(args.Context).SendToAsync(args.Context.Channel);
                    else
                        await args.Context.Channel.SendMessageAsync(tag.FormatContent(args.Context));
                }
        }
    }

    private async Task OnCommandAsync(CommandCalledEventArgs args)
    {
        Gommon.Optional<ResultCompletionData> data;
        switch (args.Result)
        {
            case ActionResult actionRes:
            {
                data = await actionRes.ExecuteResultAsync(args.Context);
                Debug(LogSource.Service,
                    $"Executed {args.Context.Command.Name}'s resulting {actionRes.GetType().AsPrettyString()}.");

                if (actionRes is BadRequestResult badreq)
                {
                    UnsavedFailedCommandCalls += 1;
                    OnBadRequest(new CommandBadRequestEventArgs(badreq, data, args));
                    return;
                }

                break;
            }

            case FailedResult failedRes:
            {
                UnsavedFailedCommandCalls += 1;
                await OnCommandFailureAsync(new CommandFailedEventArgs(failedRes, args));
                return;
            }

            default:
            {
                Critical(LogSource.Volte, "---------- IMPORTANT ----------");
                Critical(LogSource.Service,
                    $"The command {args.Context.Command.Name} didn't return some form of {typeof(ActionResult)}. " +
                    "This is developer error. " +
                    "Please report this to my developers: https://github.com/Polyhaze/Volte. Thank you!");
                Critical(LogSource.Volte, "---------- IMPORTANT ----------");
                return;
            }
        }

        UnsavedSuccessfulCommandCalls += 1;
        if (!Config.LogAllCommands) return;

        var sb = new StringBuilder()
            .AppendLine(args.FormatInvocator())
            .AppendLine(args.FormatTargetCommand())
            .AppendLine(args.FormatInvocationMessage())
            .AppendLine(args.FormatSourceGuild())
            .AppendLine(args.FormatSourceChannel())
            .AppendLine(args.FormatTimestamp())
            .AppendLine(args.ExecutedLogMessage())
            .AppendLine(args.FormatTimeTaken());
        
        if (data)
            sb.AppendLine(ResultMessage(data));

        sb.Append(CommandEventArgs.Separator);
        Info(LogSource.Volte, sb.ToString());
    }

    private static async Task OnCommandFailureAsync(CommandFailedEventArgs args)
    {
        var reason = args.Result switch
        {
            CommandNotFoundResult => "Unknown command.",
            ChecksFailedResult cfr => checksFailed(cfr),
            ParameterChecksFailedResult pcfr => paramChecksFailed(pcfr),
            ArgumentParseFailedResult apfr => $"Parsing for arguments failed for {Format.Bold(apfr.Command.Name)}.",
            TypeParseFailedResult tpfr => tpfr.FailureReason,
            OverloadsFailedResult => "A suitable overload could not be found for the given parameter type/order.",
            CommandExecutionFailedResult cefr => executionFailed(cefr),
            _ => unknown(args.Result)
        };

        if (!reason.IsNullOrEmpty())
        {
            await args.Context.CreateEmbedBuilder()
                .AddField("Error in Command", args.Context.Command.Name)
                .AddField("Error Reason", reason)
                .AddField("Usage", TextCommandHelper.FormatUsage(args.Context, args.Context.Command))
                .SendToAsync(args.Context.Channel);

            if (!Config.LogAllCommands) return;

            Error(LogSource.Module, new StringBuilder()
                .AppendLine(args.FormatInvocator())
                .AppendLine(args.FormatTargetCommand())
                .AppendLine(args.FormatInvocationMessage())
                .AppendLine(args.FormatSourceGuild())
                .AppendLine(args.FormatSourceChannel())
                .AppendLine(args.FormatTimestamp())
                .AppendLine(args.ExecutedLogMessage(reason))
                .AppendLine(args.FormatTimeTaken())
                .Append(CommandEventArgs.Separator).ToString());
        }

        return;


        static string checksFailed(ChecksFailedResult result) 
            => String(sb => sb
                .Append("One or more checks failed for command ")
                .Append(Format.Bold(result.Command.Name))
                .AppendLine(":")
                .Append(Format.Code(result.FailedChecks.Select(x => 
                    $"{x.Check.GetType().AsPrettyString().Replace("Attribute", "")}: {x.Result.FailureReason}"
                ).JoinToString('\n'), "css"))
            );


        static string paramChecksFailed(ParameterChecksFailedResult result) 
            => String(sb => sb
                .Append("One or more checks failed on parameter ")
                .Append(Format.Bold(result.Parameter.Name))
                .AppendLine(":")
                .Append(Format.Code(result.FailedChecks.Select(x => 
                    $"{x.Check.GetType().AsPrettyString().Replace("Attribute", "")}: {x.Result.FailureReason}"
                ).JoinToString('\n'), "css"))
            );
        
        static string unknown(FailedResult result)
        {
            Verbose(LogSource.Service,
                $"A command returned an unknown error. Please screenshot this message and show it to my developers: {result.GetType().AsPrettyString()}->{result.FailureReason}");
            return "Unknown error.";
        }

        static string executionFailed(CommandExecutionFailedResult result)
        {
            Error(result.Exception);
            return $"Execution of this command failed. Exception: {result.Exception.GetType().AsPrettyString()}";
        }
    }

    private static void OnBadRequest(CommandBadRequestEventArgs args)
    {
        var sb = new StringBuilder()
            .AppendLine(args.FormatInvocator())
            .AppendLine(args.FormatTargetCommand())
            .AppendLine(args.FormatInvocationMessage())
            .AppendLine(args.FormatSourceGuild())
            .AppendLine(args.FormatSourceChannel())
            .AppendLine(args.FormatTimestamp())
            .AppendLine(args.FormatResult())
            .AppendLine(args.FormatTimeTaken())
            .AppendLine(args.FormatCommandResultMessage());

        sb.Append(CommandEventArgs.Separator);
        Error(LogSource.Module, sb.ToString());
    }

    private static string ResultMessage(ResultCompletionData data) 
        => new StringBuilder(CommandEventArgs.Whitespace)
            .Append($"|     -Result Message: {data.Message?.Id}")
            .ToString();
}