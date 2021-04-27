using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class CommandsService : IVolteService
    {
        public ulong SuccessfulCommandCalls { get; private set; }
        public ulong FailedCommandCalls { get; private set; }


        public CommandsService()
        {
            SuccessfulCommandCalls = 0;
            FailedCommandCalls = 0;
        }

        public async Task OnCommandAsync(CommandCalledEventArgs args)
        {
            ResultCompletionData data;
            switch (args.Result)
            {
                case ActionResult actionRes:
                {
                    data = await actionRes.ExecuteResultAsync(args.Context);
                    Logger.Debug(LogSource.Service,
                        $"Executed {args.Context.Command.Name}'s resulting {actionRes.GetType().AsPrettyString()}.");

                    if (actionRes is BadRequestResult badreq)
                    {
                        FailedCommandCalls += 1;
                        OnBadRequest(new CommandBadRequestEventArgs(badreq, data, args));
                        return;
                    }

                    break;
                }

                case FailedResult failedRes:
                {
                    FailedCommandCalls += 1;
                    await OnCommandFailureAsync(new CommandFailedEventArgs(failedRes, args));
                    return;
                }

                default:
                {
                    Logger.Error(LogSource.Service,
                        $"The command {args.Context.Command.Name} didn't return some form of ActionResult. " +
                        "This is developer error. " +
                        "Please report this to Volte's developers: https://github.com/Ultz/Volte. Thank you!");
                    return;
                }
            }

            SuccessfulCommandCalls += 1;
            if (!Config.LogAllCommands) return;

            var sb = new StringBuilder()
                .AppendLine(CommandFrom(args))
                .AppendLine(CommandIssued(args))
                .AppendLine(FullMessage(args))
                .AppendLine(InGuild(args))
                .AppendLine(InChannel(args))
                .AppendLine(TimeIssued(args))
                .AppendLine(args.ExecutedLogMessage())
                .AppendLine(After(args));
            if (data != null)
                sb.AppendLine(ResultMessage(data));

            sb.Append(Separator);
            Logger.Info(LogSource.Volte, sb.ToString());
        }

        private async Task OnCommandFailureAsync(CommandFailedEventArgs args)
        {
            var reason = args.Result switch
            {
                CommandNotFoundResult _ => "Unknown command.",
                ChecksFailedResult cfr =>
                    $"One or more checks failed for command **{cfr.Command.Name}**: {Format.Code(cfr.FailedChecks.Select(x => x.Result.FailureReason).Join('\n'), "css")}",
                ParameterChecksFailedResult pcfr =>
                    $"One or more checks failed on parameter **{pcfr.Parameter.Name}**: {Format.Code(pcfr.FailedChecks.Select(x => x.Result.FailureReason).Join('\n'), "css")}",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed for {Format.Bold(apfr.Command.Name)}.",
                TypeParseFailedResult tpfr => tpfr.FailureReason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                CommandExecutionFailedResult cefr => ExecutionFailed(cefr),
                _ => Unknown(args.Result)
            };

            static string Unknown(FailedResult result)
            {
                Logger.Verbose(LogSource.Service,
                    $"A command returned an unknown error. Please screenshot this message and show it to my developers: {result.GetType().AsPrettyString()}");
                return "Unknown error.";
            }

            static string ExecutionFailed(CommandExecutionFailedResult result)
            {
                Logger.Exception(result.Exception);
                return $"Execution of this command failed. Exception: {result.Exception.GetType().AsPrettyString()}";
            }

            if (!reason.IsNullOrEmpty())
            {
                await args.Context.CreateEmbedBuilder()
                    .AddField("Error in Command", args.Context.Command.Name)
                    .AddField("Error Reason", reason)
                    .AddField("Usage", CommandHelper.FormatUsage(args.Context, args.Context.Command))
                    .SendToAsync(args.Context.Channel);

                if (!Config.LogAllCommands) return;

                Logger.Error(LogSource.Module, new StringBuilder()
                    .AppendLine(CommandFrom(args))
                    .AppendLine(CommandIssued(args))
                    .AppendLine(FullMessage(args))
                    .AppendLine(InGuild(args))
                    .AppendLine(InChannel(args))
                    .AppendLine(TimeIssued(args))
                    .AppendLine(args.ExecutedLogMessage(reason))
                    .AppendLine(After(args))
                    .Append(Separator).ToString());
            }
        }

        private void OnBadRequest(CommandBadRequestEventArgs args)
        {
            var sb = new StringBuilder()
                .AppendLine(CommandFrom(args))
                .AppendLine(CommandIssued(args))
                .AppendLine(FullMessage(args))
                .AppendLine(InGuild(args))
                .AppendLine(InChannel(args))
                .AppendLine(TimeIssued(args))
                .AppendLine(args.ExecutedLogMessage())
                .AppendLine(After(args));

            if (args.ResultCompletionData != null)
                sb.AppendLine(ResultMessage(args.ResultCompletionData));

            sb.Append(Separator);
            Logger.Error(LogSource.Module, sb.ToString());
        }

        private const int SpaceCount = 20;

        public static string Separator => new StringBuilder(" ".Repeat(SpaceCount)).Append("-".Repeat(49)).ToString();

        private string CommandFrom(CommandEventArgs args) => 
            $"|  -Command from user: {args.Context.User} ({args.Context.User.Id})";

        private string CommandIssued(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|     -Command Issued: {args.Context.Command.Name}").ToString();

        private string FullMessage(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|       -Full Message: {args.Context.Message.Content}").ToString();

        private string InGuild(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})").ToString();

        private string InChannel(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})").ToString();

        private string TimeIssued(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}").ToString();

        private string After(CommandEventArgs args) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|              -After: {args.Stopwatch.Elapsed.Humanize()}").ToString();

        private string ResultMessage(ResultCompletionData data) => new StringBuilder(" ".Repeat(SpaceCount)).Append(
            $"|     -Result Message: {data.Message?.Id}").ToString();
    }
}