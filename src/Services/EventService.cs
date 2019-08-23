using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

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
        private readonly CommandsService _commandsService;

        private readonly bool _shouldStream =
            !Config.Streamer.ContainsIgnoreCase(" ") || !Config.Streamer.IsNullOrEmpty();

        public EventService(LoggingService loggingService,
            DatabaseService databaseService,
            AntilinkService antilinkService,
            BlacklistService blacklistService,
            PingChecksService pingChecksService,
            CommandService commandService,
            CommandsService commandsService)
        {
            _logger = loggingService;
            _antilink = antilinkService;
            _db = databaseService;
            _blacklist = blacklistService;
            _pingchecks = pingChecksService;
            _commandService = commandService;
            _commandsService = commandsService;
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

                sw.Stop();
                await _commandsService.OnCommandAsync(new CommandCalledEventArgs(result, args.Context, sw));

                if (args.Data.Configuration.DeleteMessageOnCommand)
                    try
                    {
                        await args.Context.Message.DeleteAsync();
                    }
                    catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
                    {
                        _logger.Warn(LogSource.Service, $"Could not act upon the DeleteMessageOnCommand setting for {args.Context.Guild.Name} as the bot is missing the required permission.");
                    }
            }
        }

        public async Task OnReady(ReadyEventArgs args)
        {
            var guilds = args.Client.Guilds.Count;
            var users = args.Client.Guilds.SelectMany(x => x.Users).DistinctBy(x => x.Id).Count();
            var channels = args.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count();

            _logger.PrintVersion();
            _logger.Info(LogSource.Volte, "Use this URL to invite me to your guilds:");
            _logger.Info(LogSource.Volte, $"{args.Client.GetInviteUrl()}");
            _logger.Info(LogSource.Volte, $"Logged in as {args.Client.CurrentUser}");
            _logger.Info(LogSource.Volte, "Connected to:");
            _logger.Info(LogSource.Volte, $"    {"guild".ToQuantity(guilds)}");
            _logger.Info(LogSource.Volte, $"    {"user".ToQuantity(users)}");
            _logger.Info(LogSource.Volte, $"    {"channel".ToQuantity(channels)}");

            if (_shouldStream)
            {
                await args.Client.SetGameAsync(Config.Game);
                _logger.Info(LogSource.Volte, $"Set the bot's game to {Config.Game}.");
            }
            else
            {
                await args.Client.SetGameAsync(Config.Game, Config.FormattedStreamUrl, ActivityType.Streaming);
                _logger.Info(LogSource.Volte,
                    $"Set the bot's activity to \"{ActivityType.Streaming} {Config.Game}, at {Config.FormattedStreamUrl}\".");
            }

            foreach (var guild in args.Client.Guilds)
            {
                if (Config.BlacklistedOwners.Contains(guild.OwnerId))
                {
                    _logger.Warn(LogSource.Volte,
                        $"Left guild \"{guild.Name}\" owned by blacklisted owner {guild.Owner}.");
                    await guild.LeaveAsync();
                }

                _ = _db.GetData(guild); //ensuring all guilds have data available, to prevent exceptions later on 
            }

            if (Config.GuildLogging.EnsureValidConfiguration(args.ShardedClient, out var channel))
            {
                await new EmbedBuilder()
                    .WithSuccessColor()
                    .WithAuthor(args.ShardedClient.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is starting at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**!")
                    .SendToAsync(channel);
            }
        }
    }
}