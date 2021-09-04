using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Entities;
using Volte.Helpers;
using Volte.Services;

namespace Volte.Interactions
{
    public class CommandUpdatingService
    {
        private readonly InteractionService _interaction;
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _provider;
        private readonly DatabaseService _db;
        private bool _forcedUpdateAllCommands;
        public static bool ForceUpdateAllCommands { get; set; }

        public CommandUpdatingService(InteractionService interactionService,
            IServiceProvider provider)
        {
            _interaction = interactionService;
            _client = provider.Get<DiscordShardedClient>();
            _provider = provider;
            _db = provider.Get<DatabaseService>();
        }

        public Task InitAsync()
        {
            _client.ShardReady += async c =>
            {
                if (!_forcedUpdateAllCommands && ForceUpdateAllCommands)
                {
                    Executor.Execute(async () =>
                    {
                        try
                        {

                            var guildCommands =
                                await Task.WhenAll(_db.AllGuilds.Select(data => UpsertGuildOnlyCommandsAsync(data.Id)));

                            if (!guildCommands.IsEmpty())
                            {
                                Logger.Info(LogSource.Rest,
                                    $"{"Guild-only command".ToQuantity(guildCommands.Sum(x => x.Count))}: {guildCommands.First().Select(x => x.Name).ToReadableString()}");
                            }

                            var regularCommands = await UpsertRegularCommandsAsync();

                            var commandStr = $"{"Global command".ToQuantity(regularCommands.Count)}: ";

                            Logger.Info(LogSource.Rest,
                                commandStr + regularCommands.Select(x => x.Name).ToReadableString());

                            _forcedUpdateAllCommands = true;
                        }
                        catch (ApplicationCommandException e)
                        {
                            Logger.Debug(LogSource.Rest, e.RequestJson);
                        }

                    });
                }

                await _db.AllGuilds.Where(x => !x.HasGuildOnlyCommands)
                    .ForEachAsync(data => UpsertMissingCommandsAsync(data.Id));
            };

            _client.JoinedGuild += async g => await UpsertMissingCommandsAsync(g.Id);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<IApplicationCommand>> UpsertMissingCommandsAsync(ulong guildId)
        {
            var data = await _db.GetDataAsync(guildId);
            if (!data.HasGuildOnlyCommands)
            {
                var cmds = await UpsertGuildOnlyCommandsAsync(guildId);
                data.HasGuildOnlyCommands = true;
                _db.Save(data);
                return cmds;
            }

            return Array.Empty<IApplicationCommand>();
        }

        public Task<IReadOnlyCollection<RestGlobalCommand>> UpsertRegularCommandsAsync()
            => _client.Rest.BulkOverwriteGlobalCommands(
                _interaction.AllRegisteredCommands
                    .Where(x => !x.IsLockedToGuild)
                    .GetCommandBuilders(_provider)
            );


        public Task<IReadOnlyCollection<RestGuildCommand>> UpsertGuildOnlyCommandsAsync(ulong guildId) =>
            _client.Rest.BulkOverwriteGuildCommands(
                _interaction.AllRegisteredCommands
                    .Where(x => x.IsLockedToGuild)
                    .GetCommandBuilders(_provider), guildId
            );

        public Task<IReadOnlyCollection<RestGuildCommand>[]> UpsertMissingGuildCommandsAsync()
            => Task.WhenAll(_client.Guilds.Select(x => x.Id).Select(UpsertGuildOnlyCommandsAsync));
    }
}