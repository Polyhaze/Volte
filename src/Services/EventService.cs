using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Services
{
    public sealed class EventService : VolteService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;
        private readonly AntilinkService _antilink;
        private readonly BlacklistService _blacklist;
        private readonly PingChecksService _pingchecks;
        private readonly CommandService _commandService;

        private readonly bool _shouldStream =
            !Config.Streamer.EqualsIgnoreCase("streamer here") || !Config.Streamer.IsNullOrWhitespace();

        public EventService(LoggingService loggingService,
            DatabaseService databaseService,
            AntilinkService antilinkService,
            BlacklistService blacklistService,
            PingChecksService pingChecksService,
            CommandService commandService)
        {
            _logger = loggingService;
            _antilink = antilinkService;
            _db = databaseService;
            _blacklist = blacklistService;
            _pingchecks = pingChecksService;
            _commandService = commandService;
        }

        public async Task HandleMessageAsync(MessageReceivedEventArgs args)
        {
            if (Config.EnabledFeatures.Blacklist)
                await _blacklist.DoAsync(args);
            if (Config.EnabledFeatures.Antilink)
                await _antilink.DoAsync(args);
            if (Config.EnabledFeatures.PingChecks)
                await _pingchecks.DoAsync(args);

            var prefixes = new[]
            {
                args.Data.Configuration.CommandPrefix, $"<@{args.Context.Client.CurrentUser.Id}> ",
                $"<@!{args.Context.Client.CurrentUser.Id}> "
            }.ToList();

            if (CommandUtilities.HasAnyPrefix(args.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
                out var cmd))
            {
                var sw = Stopwatch.StartNew();
                var result = await _commandService.ExecuteAsync(cmd, args.Context, args.Context.ServiceProvider);

                if (result is CommandNotFoundResult) return;
                var targetCommand = _commandService.GetAllCommands()
                                        .FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd))
                                    ?? _commandService.GetAllCommands()
                                        .FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd.Split(' ')[0]));

                sw.Stop();
                await OnCommandAsync(targetCommand, result, args.Context, sw);

                if (args.Data.Configuration.DeleteMessageOnCommand) await args.Context.Message.DeleteAsync();
            }
        }

        public async Task OnReady(ReadyEventArgs args)
        {
            var guilds = args.Client.Guilds.Count;
            var users = args.Client.Guilds.SelectMany(x => x.Users).DistinctBy(x => x.Id).Count();
            var channels = args.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count();

            _logger.PrintVersion();
            _logger.Log(LogSeverity.Info, LogSource.Volte, "Use this URL to invite me to your servers:");
            _logger.Log(LogSeverity.Info, LogSource.Volte, $"{args.Client.GetInviteUrl()}");
            _logger.Log(LogSeverity.Info, LogSource.Volte, $"Logged in as {args.Client.CurrentUser}");
            _logger.Log(LogSeverity.Info, LogSource.Volte, "Connected to:");
            _logger.Log(LogSeverity.Info, LogSource.Volte, $"    {"guild".ToQuantity(guilds)}");
            _logger.Log(LogSeverity.Info, LogSource.Volte, $"    {"user".ToQuantity(users)}");
            _logger.Log(LogSeverity.Info, LogSource.Volte, $"    {"channel".ToQuantity(channels)}");

            if (_shouldStream)
            {
                await args.Client.SetGameAsync(Config.Game);
                _logger.Log(LogSeverity.Info, LogSource.Volte, $"Set the bot's game to {Config.Game}.");
            }
            else
            {
                await args.Client.SetGameAsync(Config.Game, Config.FormattedStreamUrl, ActivityType.Streaming);
                _logger.Log(LogSeverity.Info, LogSource.Volte,
                    $"Set the bot's activity to \"{ActivityType.Streaming} {Config.Game}, at {Config.FormattedStreamUrl}\".");
            }

            foreach (var guild in args.Client.Guilds)
            {
                if (Config.BlacklistedOwners.Contains(guild.OwnerId))
                {
                    _logger.Log(LogSeverity.Warning, LogSource.Volte,
                        $"Left guild \"{guild.Name}\" owned by blacklisted owner {guild.Owner}.");
                    await guild.LeaveAsync();
                }

                _ = _db.GetData(guild); //ensuring all guilds have data available, to prevent exceptions later on 
            }
        }

        private async Task OnCommandAsync(Command c, IResult res, ICommandContext context, Stopwatch sw)
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
                    _logger.Log(LogSeverity.Debug, LogSource.Volte,
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
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|     -Command Issued: {c.Name}");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|        -Args Passed: {args}");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|        -Time Issued: {DateTime.Now}");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|           -Executed: {res.IsSuccessful} ");
                _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|              -After: {sw.Elapsed.Humanize()}");
                if (!(data is null))
                {
                    _logger.Log(LogSeverity.Info, LogSource.Module,
                        $"|              -Result Message: {data?.Message?.Id}");
                }

                _logger.Log(LogSeverity.Info, LogSource.Module,
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
                ChecksFailedResult _ => "Insufficient permission.",
                ParameterChecksFailedResult pcfr => $"Checks failed on parameter *{pcfr.Parameter.Name}**.",
                ArgumentParseFailedResult apfr => $"Parsing for arguments failed on argument **{apfr.Parameter.Name}**."
                ,
                TypeParseFailedResult tpfr => tpfr.Reason,
                OverloadsFailedResult _ => "A suitable overload could not be found for the given parameter type/order.",
                _ => "Unknown error."
                };

            if (res is ExecutionFailedResult efr2)
                _logger.Log(LogSeverity.Error, LogSource.Module, string.Empty, efr2.Exception);

            if (!(res is CommandNotFoundResult) && !(res is ChecksFailedResult))
            {
                await embed.AddField("Error in Command:", c.Name)
                    .AddField("Error Reason:", reason)
                    .AddField("Correct Usage", c.SanitizeRemarks(ctx))
                    .WithAuthor(ctx.User)
                    .WithErrorColor()
                    .SendToAsync(ctx.Channel);

                if (!Config.LogAllCommands) return;

                Executor.Execute(() =>
                {
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|     -Command Issued: {c.Name}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|        -Args Passed: {args.Trim()}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|        -Time Issued: {DateTime.Now}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|           -Executed: {res.IsSuccessful} | Reason: {reason}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        $"|              -After: {sw.Elapsed.Humanize()}");
                    _logger.Log(LogSeverity.Error, LogSource.Module,
                        "-------------------------------------------------");
                });
            }
        }

        public Task OnBadRequestResultAsync(Command c, BadRequestResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|     -Command Issued: {c.Name}");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|        -Args Passed: {args.Trim()}");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|        -Time Issued: {DateTime.Now}");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|           -Executed: {res.IsSuccessful} | Reason: {res.Reason}");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                $"|              -After: {sw.Elapsed.Humanize()}");
            _logger.Log(LogSeverity.Error, LogSource.Module,
                "-------------------------------------------------");
            return Task.CompletedTask;
        }
    }
}