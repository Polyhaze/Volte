using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

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
            var commandName = args.Context.Message.Content.Split(" ")[0];
            var commandArgs = args.Context.Message.Content.Replace($"{commandName}", "").Trim();
            if (string.IsNullOrEmpty(commandArgs)) commandArgs = "None";

            ResultCompletionData data = null;
            switch (args.Result)
            {
                case ActionResult actionRes:
                {
                    data = await actionRes.ExecuteResultAsync(args.Context);
                    _logger.Debug(LogSource.Volte,
                        $"Executed {args.Context.Command.Name}'s resulting ActionResult.");

                    if (actionRes is BadRequestResult badreq)
                    {
                        await OnBadRequestAsync(new CommandBadRequestEventArgs(badreq, data, args.Context, commandArgs, args.Stopwatch));
                        return;
                    }

                    break;
                }

                case FailedResult failedRes:
                    await OnCommandFailureAsync(new CommandFailedEventArgs(failedRes, args.Context, commandArgs, args.Stopwatch));
                    return;
            }


            if (!Config.LogAllCommands) return;

            Executor.Execute(() =>
            {
                SuccessfulCommandCalls += 1;
                var sb = new StringBuilder()
                    .AppendLine($"|  -Command from user: {args.Context.User} ({args.Context.User.Id})") //yes, the spaces in front of each string are indeed intentional on all lines after this
                    .AppendLine($"                    |     -Command Issued: {args.Context.Command.Name}")
                    .AppendLine($"                    |        -Args Passed: {commandArgs}".PadLeft(20))
                    .AppendLine($"                    |           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})")
                    .AppendLine($"                    |         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})")
                    .AppendLine($"                    |        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}")
                    .AppendLine($"                    |           -Executed: {args.Result.IsSuccessful}")
                    .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                    .AppendLineIf($"                    |     -Result Message: {data.Message?.Id}", data != null)
                    .Append("                    -------------------------------------------------");
                _logger.Info(LogSource.Module, sb.ToString());
            });
        }

        private async Task OnCommandFailureAsync(CommandFailedEventArgs args)
        {
            FailedCommandCalls += 1;
            var reason = args.FailedResult switch
            {
                CommandNotFoundResult _ => "Unknown command.",
                ExecutionFailedResult efr => $"Execution of this command failed. Exception: {efr.Exception.GetType()}",
                ChecksFailedResult cfr => cfr.Reason,
                ParameterChecksFailedResult pcfr => $"One or more checks failed on parameter **{pcfr.Parameter.Name}**: ```css\n{pcfr.FailedChecks.Select(x => x.Result.Reason).Join('\n')}```",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed for **{apfr.Command}**.",
                TypeParseFailedResult tpfr => tpfr.Reason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                _ => "Unknown error."
            };

            if (args.FailedResult is ExecutionFailedResult e)
                _logger.Exception(e.Exception);

            if (!(args.FailedResult is ChecksFailedResult) && !reason.IsNullOrEmpty())
            {
                await args.Context.CreateEmbedBuilder()
                    .AddField("Error in Command", args.Context.Command.Name)
                    .AddField("Error Reason", reason)
                    .AddField("Correct Usage", args.Context.Command.GetUsage(args.Context))
                    .WithErrorColor()
                    .SendToAsync(args.Context.Channel);

                if (!Config.LogAllCommands) return;

                Executor.Execute(() =>
                {
                    _logger.Error(LogSource.Module, new StringBuilder()
                        .AppendLine($"|  -Command from user: {args.Context.User} ({args.Context.User.Id})") //yes, the spaces in front of each string are indeed intentional on all lines after this
                        .AppendLine($"                    |     -Command Issued: {args.Context.Command.Name}")
                        .AppendLine($"                    |        -Args Passed: {args.Arguments.Trim()}")
                        .AppendLine($"                    |           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})")
                        .AppendLine($"                    |         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})")
                        .AppendLine($"                    |        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}")
                        .AppendLine($"                    |           -Executed: {args.FailedResult.IsSuccessful} | Reason: {reason}")
                        .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                        .Append("                    -------------------------------------------------").ToString());
                });
            }
        }

        public Task OnBadRequestAsync(CommandBadRequestEventArgs args)
        {
            FailedCommandCalls += 1;
            Executor.Execute(() =>
            {
                _logger.Error(LogSource.Module, new StringBuilder()
                    .AppendLine($"|  -Command from user: {args.Context.User} ({args.Context.User.Id})") //yes, the spaces in front of each string are indeed intentional on all lines after this
                    .AppendLine($"                    |     -Command Issued: {args.Context.Command.Name}")
                    .AppendLine($"                    |        -Args Passed: {args.Arguments.Trim()}")
                    .AppendLine($"                    |           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})")
                    .AppendLine($"                    |         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})")
                    .AppendLine($"                    |        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}")
                    .AppendLine($"                    |           -Executed: {args.BadRequestResult.IsSuccessful} | Reason: {args.BadRequestResult.Reason}")
                    .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                    .AppendLineIf($"                    |     -Result Message: {args.ResultCompletionData.Message?.Id}", args.ResultCompletionData != null)
                    .Append("                    -------------------------------------------------").ToString());
            });
            return Task.CompletedTask;
        }
    }
}
