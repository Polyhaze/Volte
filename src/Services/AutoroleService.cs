﻿using System.Threading.Tasks;
using Discord.WebSocket;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Services
{
    public sealed class AutoroleService : IVolteService
    {
        private readonly DatabaseService _db;

        public AutoroleService(DiscordShardedClient client, DatabaseService databaseService)
        {
            _db = databaseService;
            client.UserJoined += u =>
                Config.EnabledFeatures.Autorole
                    ? ApplyRoleAsync(new UserJoinedEventArgs(u))
                    : Task.CompletedTask;
        }

        public async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            var targetRole = args.Guild.GetRole(data.Configuration.Autorole);
            if (targetRole is null)
            {
                Logger.Debug(LogSource.Volte,
                    $"Guild {args.Guild.Name}'s Autorole is set to an ID of a role that no longer exists; or is not set at all.");
                return;
            }

            await args.User.AddRoleAsync(targetRole, DiscordHelper.CreateRequestOptions(x => x.AuditLogReason = "Volte Autorole"));
            Logger.Debug(LogSource.Volte,
                $"Applied role {targetRole.Name} to user {args.User} in guild {args.Guild.Name}.");
        }
    }
}