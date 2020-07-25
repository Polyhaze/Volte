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
                        await OnBadRequestAsync(new CommandBadRequestEventArgs(badreq, data, args.Context, args.Arguments, args.Stopwatch));
                        return;
                    }

                    break;
                }

                case FailedResult failedRes:
                {
                    await OnCommandFailureAsync(new CommandFailedEventArgs(failedRes, args.Context, args.Arguments, args.Stopwatch));
                    return;
                }

                default:
                {
                    _logger.Error(LogSource.Service, 
                        $"The command {args.Context.Command.Name} didn't return some form of ActionResult. " +
                        "This is developer error. " +
                        "Please report this to Volte's developers: https://github.com/Ultz/Volte. Thank you!");
                    break;
                }
                    
            }


            if (!Config.LogAllCommands) return;

            // ReSharper disable once PossibleNullReferenceException
            // this possible error occurs in line 85 but it cannot happen because it's in the AppendLineIf method.
            // This is just to make Rider shut the hell up.
            Executor.Execute(() =>
            {
                SuccessfulCommandCalls += 1;
                var sb = new StringBuilder()
                    .AppendLine($"|  -Command from user: {args.Context.User} ({args.Context.User.Id})") //yes, the spaces in front of each string are indeed intentional on all lines after this
                    .AppendLine($"                    |     -Command Issued: {args.Context.Command.Name}")
                    .AppendLine($"                    |        -Args Passed: {args.Arguments}".PadLeft(20))
                    .AppendLine($"                    |           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})")
                    .AppendLine($"                    |         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})")
                    .AppendLine($"                    |        -Time Issued: {args.Context.Now.FormatFullTime()}, {args.Context.Now.FormatDate()}")
                    .AppendLine($"                    |           -Executed: {args.Result.IsSuccessful}")
                    .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                    .AppendLineIf($"                    |     -Result Message: {data.Message?.Id}", data != null)
                    .Append("                    -------------------------------------------------");
                _logger.Info(LogSource.Volte, sb.ToString());
            });
        }

        private async Task OnCommandFailureAsync(CommandFailedEventArgs args)
        {
            FailedCommandCalls += 1;
            var reason = args.Result switch
            {
                CommandNotFoundResult _ => "Unknown command.",
                ExecutionFailedResult efr => $"Execution of this command failed. Exception: {efr.Exception.GetType()}",
                ChecksFailedResult cfr => cfr.Reason,
                ParameterChecksFailedResult pcfr => $"One or more checks failed on parameter **{pcfr.Parameter.Name}**: ```css\n{pcfr.FailedChecks.Select(x => x.Result.Reason).Join('\n')}```",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed for **{apfr.Command}**.",
                TypeParseFailedResult tpfr => tpfr.Reason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                _ => Unknown(args.Result)
            };

            string Unknown(FailedResult result)
            {
                _logger.Verbose(LogSource.Service, $"A command returned an unknown error. Please screenshot this message and show it to my developers: {result.GetType().Name}");
                return "Unknown error.";
            }
            

            if (args.Result is ExecutionFailedResult e)
                _logger.Exception(e.Exception);

            if (!reason.IsNullOrEmpty())
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
                        .AppendLine($"                    |           -Executed: {args.Result.IsSuccessful} | Reason: {reason}")
                        .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                        .Append("                    -------------------------------------------------").ToString());
                });
            }
        }

        private Task OnBadRequestAsync(CommandBadRequestEventArgs args)
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
                    .AppendLine($"                    |           -Executed: {args.Result.IsSuccessful} | Reason: {args.Result.Reason}")
                    .AppendLine($"                    |              -After: {args.Stopwatch.Elapsed.Humanize()}")
                    .AppendLineIf($"                    |     -Result Message: {args.ResultCompletionData.Message?.Id}", args.ResultCompletionData != null)
                    .Append("                    -------------------------------------------------").ToString());
            });
            return Task.CompletedTask;
        }
    }
}
