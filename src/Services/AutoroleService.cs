using System;
using System.Threading.Tasks;
using Gommon;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class AutoroleService : VolteService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;

        public AutoroleService(LoggingService loggingService,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _db = databaseService;
        }

        public async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            var targetRole = args.Guild.GetRole(data.Configuration.Autorole);
            if (targetRole is null)
            {
                _logger.Debug(LogSource.Volte,
                    $"Guild {args.Guild.Name}'s Autorole is set to an ID of a role that no longer exists; or is not set at all.");
                return;
            }

            await args.User.AddRoleAsync(targetRole);
            _logger.Debug(LogSource.Volte,
                $"Applied role {targetRole.Name} to user {args.User} in guild {args.Guild.Name}.");
        }
    }
}