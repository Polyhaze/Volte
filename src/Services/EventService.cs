using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Volte.Data;
using Volte.Data.Objects;
using Volte.Data.Objects.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Event", "The main Service for handling some Discord gateway events.")]
    public sealed class EventService
    {
        private readonly LoggingService _logger;

        private readonly bool _shouldStream =
            Config.Streamer.EqualsIgnoreCase("streamer here") || Config.Streamer.IsNullOrWhitespace();

        public EventService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        public async Task OnReady(ReadyEventArgs args)
        {
            await _logger.Log(LogSeverity.Info, LogSource.Volte, $"Currently running Volte V{Version.FullVersion}");
            await _logger.Log(LogSeverity.Info, LogSource.Volte, "Use this URL to invite me to your servers:");
            await _logger.Log(LogSeverity.Info, LogSource.Volte, $"{args.Client.GetInviteUrl(true)}");
            await _logger.Log(LogSeverity.Info, LogSource.Volte, $"Logged in as {args.Client.CurrentUser}");
            await _logger.Log(LogSeverity.Info, LogSource.Volte, "Connected to:");
            await _logger.Log(LogSeverity.Info, LogSource.Volte, $"    {args.Client.Guilds.Count} servers");
            await _logger.Log(LogSeverity.Info, LogSource.Volte,
                $"    {args.Client.Guilds.SelectMany(x => x.Users).DistinctBy(x => x.Id).Count()} user");
            await _logger.Log(LogSeverity.Info, LogSource.Volte,
                $"    {args.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count()} channels");


            if (_shouldStream)
            {
                await args.Client.SetGameAsync(Config.Game);
                await _logger.Log(LogSeverity.Info, LogSource.Volte, $"Set the bot's game to {Config.Game}.");
            }
            else
            {
                var twitchUrl = $"https://twitch.tv/{Config.Streamer}";
                await args.Client.SetGameAsync(Config.Game, twitchUrl, ActivityType.Streaming);
                await _logger.Log(LogSeverity.Info, LogSource.Volte,
                    $"Set the bot's game to \"{ActivityType.Streaming} {Config.Game}, at {twitchUrl}\".");
            }

            foreach (var guild in args.Client.Guilds)
            {
                if (!Config.BlacklistedOwners.Contains(guild.OwnerId)) continue;
                await _logger.Log(LogSeverity.Warning, LogSource.Volte,
                    $"Left guild \"{guild.Name}\" owned by blacklisted owner {guild.Owner}.");
                await guild.LeaveAsync();
            }
        }

        public async Task OnCommandAsync(Command c, IResult res, ICommandContext context, Stopwatch sw)
        {
            var ctx = (VolteContext) context;
            var commandName = ctx.Message.Content.Split(" ")[0];
            var args = ctx.Message.Content.Replace($"{commandName}", "");
            if (string.IsNullOrEmpty(args)) args = "None";
            if (res is FailedResult failedRes)
            {
                await OnCommandFailureAsync(c, failedRes, ctx, args, sw);
                return;
            }

            if (Config.LogAllCommands)
            {
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|     -Command Issued: {c.Name}");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|        -Args Passed: {args.Trim()}");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|        -Time Issued: {DateTime.Now}");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|           -Executed: {res.IsSuccessful} ");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    $"|              -After: {sw.Elapsed.Humanize()}");
                await _logger.Log(LogSeverity.Info, LogSource.Module,
                    "-------------------------------------------------");
            }
        }

        private async Task OnCommandFailureAsync(Command c, FailedResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            var embed = new EmbedBuilder();
            string reason;
            switch (res)
            {
                case CommandNotFoundResult _:
                    reason = "Unknown command.";
                    break;
                case ExecutionFailedResult efr:
                    reason = $"Execution of this command failed.\nFull error message: {efr.Exception.Message}";
                    await _logger.Log(LogSeverity.Error, LogSource.Module, string.Empty, efr.Exception);
                    break;
                case ChecksFailedResult _:
                    reason = "Insufficient permission.";
                    break;
                case ParameterChecksFailedResult pcfr:
                    reason = $"Checks failed on parameter *{pcfr.Parameter.Name}**.";
                    break;
                case ArgumentParseFailedResult apfr:
                    reason = $"Parsing for arguments failed on argument **{apfr.Parameter?.Name}**.";
                    break;
                case TypeParseFailedResult tpfr:
                    reason =
                        $"Failed to parse type **{tpfr.Parameter.Type}** from parameter **{tpfr.Parameter.Name}**.";
                    break;
                default:
                    reason = "Unknown error.";
                    break;
            }

            if (reason != "Insufficient permission." && reason != "Unknown command.")
            {
                await embed.AddField("Error in Command:", c.Name)
                    .AddField("Error Reason:", reason)
                    .AddField("Correct Usage", c.SanitizeRemarks(ctx))
                    .WithAuthor(ctx.User)
                    .WithErrorColor()
                    .SendToAsync(ctx.Channel);

                if (!Config.LogAllCommands) return;

                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|     -Command Issued: {c.Name}");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|        -Args Passed: {args.Trim()}");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|        -Time Issued: {DateTime.Now}");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|           -Executed: {res.IsSuccessful} | Reason: {reason}");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    $"|              -After: {sw.Elapsed.Humanize()}");
                await _logger.Log(LogSeverity.Error, LogSource.Module,
                    "-------------------------------------------------");
            }
        }
    }
}