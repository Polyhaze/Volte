using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using SixLabors.ImageSharp;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class CommandsService : VolteService
    {
        public ulong SuccessfulCommandCalls { get; private set; }
        public ulong FailedCommandCalls { get; private set; }

        private readonly LoggingService _logger;

        public CommandsService(LoggingService loggingService)
        {
            _logger = loggingService;
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
                    _logger.Debug(LogSource.Volte,
                        $"Executed {args.Context.Command.Name}'s resulting ActionResult.");

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
                    _logger.Error(LogSource.Service, 
                        $"The command {args.Context.Command.Name} didn't return some form of ActionResult. " +
                        "This is developer error. " +
                        "Please report this to Volte's developers: https://github.com/Ultz/Volte. Thank you!");
                    data = null;
                    return;
                }
                    
            }

            SuccessfulCommandCalls += 1;
            if (!Config.LogAllCommands) return;
            
            var sb = new StringBuilder()
                .AppendLine(CommandFrom(args))
                .AppendLine(CommandIssued(args))
                .AppendLine(ArgsPassed(args))
                .AppendLine(InGuild(args))
                .AppendLine(InChannel(args))
                .AppendLine(TimeIssued(args))
                .AppendLine(args.ExecutedLogMessage())
                .AppendLine(After(args));
            if (data != null)
            {
                sb.AppendLine(ResultMessage(data));
            }
            sb.Append(_separator);
            _logger.Info(LogSource.Volte, sb.ToString());
            }

        private async Task OnCommandFailureAsync(CommandFailedEventArgs args)
        {
            var reason = args.Result switch
            {
                CommandNotFoundResult _ => "Unknown command.",
                ChecksFailedResult cfr => $"One or more checks failed for command **{cfr.Command.Name}**: ```css\n{cfr.FailedChecks.Select(x => x.Result.FailureReason).Join('\n')}```",
                ParameterChecksFailedResult pcfr => $"One or more checks failed on parameter **{pcfr.Parameter.Name}**: ```css\n{pcfr.FailedChecks.Select(x => x.Result.FailureReason).Join('\n')}```",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed for **{apfr.Command}**.",
                TypeParseFailedResult tpfr => tpfr.FailureReason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                ExecutionFailedResult efr => ExecutionFailed(efr),
                _ => Unknown(args.Result)
            };

            string Unknown(FailedResult result)
            {
                _logger.Verbose(LogSource.Service, $"A command returned an unknown error. Please screenshot this message and show it to my developers: {result.GetType().Name}");
                return "Unknown error.";
            }

            string ExecutionFailed(ExecutionFailedResult result)
            {
                _logger.Exception(result.Exception);
                return $"Execution of this command failed. Exception: {result.Exception.GetType()}";
            }

            if (!reason.IsNullOrEmpty())
            {
                await args.Context.CreateEmbedBuilder()
                    .AddField("Error in Command", args.Context.Command.Name)
                    .AddField("Error Reason", reason)
                    .WithErrorColor()
                    .SendToAsync(args.Context.Channel);

                if (!Config.LogAllCommands) return;
                
                _logger.Error(LogSource.Module, new StringBuilder()
                    .AppendLine(CommandFrom(args))
                    .AppendLine(CommandIssued(args))
                    .AppendLine(ArgsPassed(args))
                    .AppendLine(InGuild(args))
                    .AppendLine(InChannel(args))
                    .AppendLine(TimeIssued(args))
                    .AppendLine(args.ExecutedLogMessage(reason))
                    .AppendLine(After(args))
                    .Append(_separator).ToString());
            }
        }

        private void OnBadRequest(CommandBadRequestEventArgs args)
        {
            var sb = new StringBuilder()
                .AppendLine(CommandFrom(args))
                .AppendLine(CommandIssued(args))
                .AppendLine(ArgsPassed(args))
                .AppendLine(InGuild(args))
                .AppendLine(InChannel(args))
                .AppendLine(TimeIssued(args))
                .AppendLine(args.ExecutedLogMessage())
                .AppendLine(After(args));
            
            if (args.ResultCompletionData != null)
            {
                sb.AppendLine(ResultMessage(args.ResultCompletionData));
            }
            sb.Append(_separator);
            _logger.Error(LogSource.Module, sb.ToString());
        }

        private readonly string _separator                        = "                    -------------------------------------------------";
        private string CommandFrom(CommandEventArgs args)                           => $"|  -Command from user: {args.Context.User} ({args.Context.User.Id})"; //yes, the spaces in front of each string are indeed intentional on all lines after this
        private string CommandIssued(CommandEventArgs args)     => $"                    |     -Command Issued: {args.Context.Command.Name}";
        private string ArgsPassed(CommandEventArgs args)        => $"                    |        -Args Passed: {args.Arguments.Trim()}";
        private string InGuild(CommandEventArgs args)           => $"                    |           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})";
        private string InChannel(CommandEventArgs args)         => $"                    |         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})";
        private string TimeIssued(CommandEventArgs args)        => $"                    |        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}";
        private string After(CommandEventArgs args)             => $"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}";
        private string ResultMessage(ResultCompletionData data) => $"                    |     -Result Message: {data.Message?.Id}";
    }
    
}
