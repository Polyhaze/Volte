using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Gommon;
using Humanizer;
using Qmmands;
using Qommon.Collections;
using Volte.Core;
using Volte.Core.Entities;

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
        private readonly QuoteService _quoteService;
        private readonly AddonService _addonService;

        private readonly bool _shouldStream =
            !Config.Streamer.IsNullOrWhitespace();

        private readonly bool _shouldSetGame =
            !Config.Game.IsNullOrWhitespace();

        public EventService(LoggingService loggingService,
            DatabaseService databaseService,
            AntilinkService antilinkService,
            BlacklistService blacklistService,
            PingChecksService pingChecksService,
            CommandService commandService,
            CommandsService commandsService,
            QuoteService quoteService,
            AddonService addonService)
        {
            _logger = loggingService;
            _antilink = antilinkService;
            _db = databaseService;
            _blacklist = blacklistService;
            _pingchecks = pingChecksService;
            _commandService = commandService;
            _commandsService = commandsService;
            _quoteService = quoteService;
            _addonService = addonService;
        }

        public async Task HandleMessageAsync(MessageReceivedEventArgs args)
        {
            if (Config.EnabledFeatures.Blacklist)
                await _blacklist.CheckMessageAsync(args);
            if (Config.EnabledFeatures.Antilink)
                await _antilink.CheckMessageAsync(args);
            if (Config.EnabledFeatures.PingChecks)
                await _pingchecks.CheckMessageAsync(args);

            var prefixes = new List<string>
            {
                args.Data.Configuration.CommandPrefix, $"<@{args.Context.Client.CurrentUser.Id}> ",
                $"<@!{args.Context.Client.CurrentUser.Id}> "
            };

            if (CommandUtilities.HasAnyPrefix(args.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
                out var cmd))
            {
                var sw = Stopwatch.StartNew();
                var result = await _commandService.ExecuteAsync(cmd, args.Context);

                if (result is CommandNotFoundResult) return;

                sw.Stop();
                await _commandsService.OnCommandAsync(new CommandCalledEventArgs(result, args.Context, sw));
            }
            else
            {
                if (args.Message.Content.Equals($"<@{args.Context.Client.CurrentUser.Id}>")
                    || args.Message.Content.Equals($"<@!{args.Context.Client.CurrentUser.Id}>"))
                {
                    await new EmbedBuilder().WithColor(Config.SuccessColor).WithDescription(
                            $"The prefix for this guild is **{args.Data.Configuration.CommandPrefix}**; " +
                            $"alternatively you can just mention me as a prefix, i.e. `@{args.Context.Guild.CurrentUser} help`.")
                        .ReplyToAsync(args.Message);
                    return;
                }

                await _quoteService.CheckMessageAsync(args);
            }
        }

        public async Task OnShardReadyAsync(ShardReadyEventArgs args)
        {
            var guilds = args.Shard.Guilds.Count;
            var users = args.Shard.Guilds.SelectMany(x => x.Users).DistinctBy(x => x.Id).Count();
            var channels = args.Shard.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count();

            _logger.PrintVersion();
            _logger.Info(LogSource.Volte, "Use this URL to invite me to your guilds:");
            _logger.Info(LogSource.Volte, $"{args.Shard.GetInviteUrl()}");
            _logger.Info(LogSource.Volte, $"Logged in as {args.Shard.CurrentUser}, shard {args.Shard.ShardId}");
            _logger.Info(LogSource.Volte, $"Default command prefix is: \"{Config.CommandPrefix}\"");
            _logger.Info(LogSource.Volte, "Connected to:");
            _logger.Info(LogSource.Volte, $"     {"guild".ToQuantity(guilds)}");
            _logger.Info(LogSource.Volte, $"     {"user".ToQuantity(users)}");
            _logger.Info(LogSource.Volte, $"     {"channel".ToQuantity(channels)}");

            if (!_shouldStream)
            {
                if (_shouldSetGame)
                {
                    await args.Shard.SetGameAsync(Config.Game);
                    _logger.Info(LogSource.Volte, $"Set {args.Shard.CurrentUser.Username}'s game to \"{Config.Game}\".");
                }
            }
            else
            {
                await args.Shard.SetGameAsync(Config.Game, Config.FormattedStreamUrl, ActivityType.Streaming);
                _logger.Info(LogSource.Volte,
                    $"Set {args.Shard.CurrentUser.Username}'s activity to \"{ActivityType.Streaming}: {Config.Game}\", at Twitch user {Config.Streamer}.");
            }

            _ = Task.Run(async () =>
            {
                foreach (var guild in args.Shard.Guilds)
                {
                    if (Config.BlacklistedOwners.Contains(guild.OwnerId))
                    {
                        _logger.Warn(LogSource.Volte,
                            $"Left guild \"{guild.Name}\" owned by blacklisted owner {guild.Owner}.");
                        await guild.LeaveAsync();
                    }
                    else _db.GetData(guild); //ensuring all guilds have data available to prevent exceptions later on 
                }
            });

            if (Config.GuildLogging.EnsureValidConfiguration(args.Client, out var channel))
            {
                await new EmbedBuilder()
                    .WithSuccessColor()
                    .WithAuthor(args.Client.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is starting at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**!")
                    .SendToAsync(channel);
            }
        }
    }
}