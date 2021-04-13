using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class EventService : VolteService
    {
        private readonly DatabaseService _db;
        private readonly AntilinkService _antilink;
        private readonly BlacklistService _blacklist;
        private readonly PingChecksService _pingchecks;
        private readonly CommandService _commandService;
        private readonly CommandsService _commandsService;
        private readonly QuoteService _quoteService;

        public EventService(DatabaseService databaseService,
            AntilinkService antilinkService,
            BlacklistService blacklistService,
            PingChecksService pingChecksService,
            CommandService commandService,
            CommandsService commandsService,
            QuoteService quoteService)
        {
            _antilink = antilinkService;
            _db = databaseService;
            _blacklist = blacklistService;
            _pingchecks = pingChecksService;
            _commandService = commandService;
            _commandsService = commandsService;
            _quoteService = quoteService;
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
                    await args.Context.CreateEmbedBuilder($"The prefix for this guild is **{args.Data.Configuration.CommandPrefix}**; " +
                                                          $"alternatively you can just mention me as a prefix, i.e. `@{args.Context.Guild.CurrentUser} help`.")
                        .ReplyToAsync(args.Message);
                }
                else
                    await _quoteService.CheckMessageAsync(args);
            }
        }

        public async Task OnShardReadyAsync(ShardReadyEventArgs args)
        {
            var guilds = args.Shard.Guilds.Count;
            var users = args.Shard.Guilds.SelectMany(x => x.Users).DistinctBy(x => x.Id).Count();
            var channels = args.Shard.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count();

            Logger.PrintVersion();
            Logger.Info(LogSource.Volte, "Use this URL to invite me to your guilds:");
            Logger.Info(LogSource.Volte, $"{args.Shard.GetInviteUrl()}");
            Logger.Info(LogSource.Volte, $"Logged in as {args.Shard.CurrentUser}, shard {args.Shard.ShardId}");
            Logger.Info(LogSource.Volte, $"Default command prefix is: \"{Config.CommandPrefix}\"");
            Logger.Info(LogSource.Volte, "Connected to:");
            Logger.Info(LogSource.Volte, $"     {"guild".ToQuantity(guilds)}");
            Logger.Info(LogSource.Volte, $"     {"user".ToQuantity(users)}");
            Logger.Info(LogSource.Volte, $"     {"channel".ToQuantity(channels)}");

            var (type, name, streamer) = Config.ParseConfigActivity();

            if (streamer is null && type != ActivityType.CustomStatus)
            {
                await args.Shard.SetGameAsync(name, null, type);
                Logger.Info(LogSource.Volte, $"Set {args.Shard.CurrentUser.Username}'s game to \"{Config.Game}\".");
            }
            else if (type != ActivityType.CustomStatus)
            {
                await args.Shard.SetGameAsync(name, Config.FormattedStreamUrl, type);
                Logger.Info(LogSource.Volte,
                    $"Set {args.Shard.CurrentUser.Username}'s activity to \"{type}: {name}\", at Twitch user {Config.Streamer}.");
            }

            _ = Executor.ExecuteAsync(async () =>
            {
                foreach (var guild in args.Shard.Guilds)
                {
                    if (Config.BlacklistedOwners.Contains(guild.OwnerId))
                    {
                        Logger.Warn(LogSource.Volte,
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
                        $"Volte {Version.FullVersion} is starting {DateTime.Now.FormatBoldString()}!")
                    .SendToAsync(channel);
            }
        }
    }
}