using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
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
        private readonly LoggingService _logger;

        public CommandsService(LoggingService loggingService) 
            => _logger = loggingService;

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

                    switch (args.Result)
                    {
                        case BadRequestResult badreq:
                            await OnBadRequestResultAsync(new CommandBadRequestEventArgs(badreq, args.Context, commandArgs, args.Stopwatch));
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
                _logger.Info(LogSource.Module,
                    $"|  -Command from user: {args.Context.User.Username}#{args.Context.User.Discriminator} ({args.Context.User.Id})");
                _logger.Info(LogSource.Module,
                    $"|     -Command Issued: {args.Context.Command.Name}");
                _logger.Info(LogSource.Module,
                    $"|        -Args Passed: {commandArgs}");
                _logger.Info(LogSource.Module,
                    $"|           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})");
                _logger.Info(LogSource.Module,
                    $"|         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})");
                _logger.Info(LogSource.Module,
                    $"|        -Time Issued: {DateTimeOffset.UtcNow.FormatFullTime()}, {DateTimeOffset.UtcNow.FormatDate()}");
                _logger.Info(LogSource.Module,
                    $"|           -Executed: {args.Result.IsSuccessful} ");
                _logger.Info(LogSource.Module,
                    $"|              -After: {args.Stopwatch.Elapsed.Humanize()}");
                if (!(data is null))
                {
                    _logger.Info(LogSource.Module,
                        $"|              -Result Message: {data?.Message?.Id}");
                }

                _logger.Info(LogSource.Module,
                    "-------------------------------------------------");
            });
        }

        private async Task OnCommandFailureAsync(CommandFailedEventArgs args)
        {
            var embed = new EmbedBuilder();
            var reason = args.FailedResult switch
            {
                CommandNotFoundResult _ => "Unknown command.",
                ExecutionFailedResult efr => $"Execution of this command failed. Exception: {efr.Exception.GetType().FullName}",
                ChecksFailedResult cfr => cfr.Reason,
                ParameterChecksFailedResult pcfr => $"Checks failed on parameter *{pcfr.Parameter.Name}**.",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed on argument **{apfr.Parameter.Name}**.",
                TypeParseFailedResult tpfr => tpfr.Reason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                _ => "Unknown error."
            };

            if (args.FailedResult is ExecutionFailedResult efr2)
                _logger.Error(LogSource.Module, string.Empty, efr2.Exception);

            if (!(args.FailedResult is ChecksFailedResult))
            {
                await embed.AddField("Error in Command", args.Context.Command.Name)
                    .AddField("Error Reason", reason)
                    .AddField("Correct Usage", args.Context.Command.GetUsage(args.Context))
                    .WithAuthor(args.Context.User)
                    .WithErrorColor()
                    .SendToAsync(args.Context.Channel);

                if (!Config.LogAllCommands) return;

                Executor.Execute(() =>
                {
                    _logger.Error(LogSource.Module,
                        $"|  -Command from user: {args.Context.User.Username}#{args.Context.User.Discriminator} ({args.Context.User.Id})");
                    _logger.Error(LogSource.Module,
                        $"|     -Command Issued: {args.Context.Command.Name}");
                    _logger.Error(LogSource.Module,
                        $"|        -Args Passed: {args.Arguments.Trim()}");
                    _logger.Error(LogSource.Module,
                        $"|           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})");
                    _logger.Error(LogSource.Module,
                        $"|         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})");
                    _logger.Error(LogSource.Module,
                        $"|        -Time Issued: {DateTimeOffset.UtcNow.FormatFullTime()}, {DateTimeOffset.UtcNow.FormatDate()}");
                    _logger.Error(LogSource.Module,
                        $"|           -Executed: {args.FailedResult.IsSuccessful} | Reason: {reason}");
                    _logger.Error(LogSource.Module,
                        $"|              -After: {args.Stopwatch.Elapsed.Humanize()}");
                    _logger.Error(LogSource.Module,
                        "-------------------------------------------------");
                });
            }
        }

        public Task OnBadRequestResultAsync(CommandBadRequestEventArgs args)
        {
            Executor.Execute(() =>
            {
                _logger.Error(LogSource.Module,
                    $"|  -Command from user: {args.Context.User.Username}#{args.Context.User.Discriminator} ({args.Context.User.Id})");
                _logger.Error(LogSource.Module,
                    $"|     -Command Issued: {args.Context.Command.Name}");
                _logger.Error(LogSource.Module,
                    $"|        -Args Passed: {args.Arguments.Trim()}");
                _logger.Error(LogSource.Module,
                    $"|           -In Guild: {args.Context.Guild.Name} ({args.Context.Guild.Id})");
                _logger.Error(LogSource.Module,
                    $"|         -In Channel: #{args.Context.Channel.Name} ({args.Context.Channel.Id})");
                _logger.Error(LogSource.Module,
                    $"|        -Time Issued: {DateTimeOffset.UtcNow.FormatFullTime()}, {DateTimeOffset.UtcNow.FormatDate()}");
                _logger.Error(LogSource.Module,
                    $"|           -Executed: {args.BadRequestResult.IsSuccessful} | Reason: {args.BadRequestResult.Reason}");
                _logger.Error(LogSource.Module,
                    $"|              -After: {args.Stopwatch.Elapsed.Humanize()}");
                _logger.Error(LogSource.Module,
                    "-------------------------------------------------");
            });
            return Task.CompletedTask;
        }
    }
}
