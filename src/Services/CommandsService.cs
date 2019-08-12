using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Models;

namespace Volte.Services
{
    public sealed class CommandsService : VolteService
    {
        private readonly LoggingService _logger;

        public CommandsService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        public async Task OnCommandAsync(Command c, IResult res, CommandContext context, Stopwatch sw)
        {
            var ctx = context.Cast<VolteContext>();
            var commandName = ctx.Message.Content.Split(" ")[0];
            var args = ctx.Message.Content.Replace($"{commandName}", "").Trim();
            if (string.IsNullOrEmpty(args)) args = "None";

            ResultCompletionData data = null;
            switch (res)
            {
                case ActionResult actionRes:
                {
                    data = await actionRes.ExecuteResultAsync(ctx);
                    _logger.Debug(LogSource.Volte,
                        $"Executed {c.Name}'s resulting ActionResult.");

                    switch (res)
                    {
                        case BadRequestResult badreq:
                            await OnBadRequestResultAsync(c, badreq, ctx, args, sw);
                            return;
                    }

                    break;
                }

                case FailedResult failedRes:
                    await OnCommandFailureAsync(c, failedRes, ctx, args, sw);
                    return;
            }


            if (!Config.LogAllCommands) return;

            Executor.Execute(() =>
            {
                _logger.Info(LogSource.Module,
                    $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                _logger.Info(LogSource.Module,
                    $"|     -Command Issued: {c.Name}");
                _logger.Info(LogSource.Module,
                    $"|        -Args Passed: {args}");
                _logger.Info(LogSource.Module,
                    $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                _logger.Info(LogSource.Module,
                    $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                _logger.Info(LogSource.Module,
                    $"|        -Time Issued: {DateTime.UtcNow.Humanize()}");
                _logger.Info(LogSource.Module,
                    $"|           -Executed: {res.IsSuccessful} ");
                _logger.Info(LogSource.Module,
                    $"|              -After: {sw.Elapsed.Humanize()}");
                if (!(data is null))
                {
                    _logger.Info(LogSource.Module,
                        $"|              -Result Message: {data?.Message?.Id}");
                }

                _logger.Info(LogSource.Module,
                    "-------------------------------------------------");
            });
        }

        private async Task OnCommandFailureAsync(Command c, FailedResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            var embed = new EmbedBuilder();
            var reason = res switch
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

            if (res is ExecutionFailedResult efr2)
                _logger.Error(LogSource.Module, string.Empty, efr2.Exception);

            if (!(res is CommandNotFoundResult) && !(res is ChecksFailedResult))
            {
                await embed.AddField("Error in Command", c.Name)
                    .AddField("Error Reason", reason)
                    .AddField("Correct Usage", c.GetUsage(ctx))
                    .WithAuthor(ctx.User)
                    .WithErrorColor()
                    .SendToAsync(ctx.Channel);

                if (!Config.LogAllCommands) return;

                Executor.Execute(() =>
                {
                    _logger.Error(LogSource.Module,
                        $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                    _logger.Error(LogSource.Module,
                        $"|     -Command Issued: {c.Name}");
                    _logger.Error(LogSource.Module,
                        $"|        -Args Passed: {args.Trim()}");
                    _logger.Error(LogSource.Module,
                        $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                    _logger.Error(LogSource.Module,
                        $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                    _logger.Error(LogSource.Module,
                        $"|        -Time Issued: {DateTime.UtcNow.Humanize()}");
                    _logger.Error(LogSource.Module,
                        $"|           -Executed: {res.IsSuccessful} | Reason: {reason}");
                    _logger.Error(LogSource.Module,
                        $"|              -After: {sw.Elapsed.Humanize()}");
                    _logger.Error(LogSource.Module,
                        "-------------------------------------------------");
                });
            }
        }

        public Task OnBadRequestResultAsync(Command c, BadRequestResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            _logger.Error(LogSource.Module,
                $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
            _logger.Error(LogSource.Module,
                $"|     -Command Issued: {c.Name}");
            _logger.Error(LogSource.Module,
                $"|        -Args Passed: {args.Trim()}");
            _logger.Error(LogSource.Module,
                $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
            _logger.Error(LogSource.Module,
                $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
            _logger.Error(LogSource.Module,
                $"|        -Time Issued: {DateTime.Now}");
            _logger.Error(LogSource.Module,
                $"|           -Executed: {res.IsSuccessful} | Reason: {res.Reason}");
            _logger.Error(LogSource.Module,
                $"|              -After: {sw.Elapsed.Humanize()}");
            _logger.Error(LogSource.Module,
                "-------------------------------------------------");
            return Task.CompletedTask;
        }
    }
}
