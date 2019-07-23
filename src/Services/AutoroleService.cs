using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Data.Models;
using Volte.Core.Data.Models.EventArgs;

namespace Volte.Services
{
    [Service("Autorole", "The main Service for automatically applying roles when a user joins any given guild.")]
    public sealed class AutoroleService
    {
        private LoggingService _logger;
        private DatabaseService _db;

        public AutoroleService(LoggingService loggingService,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _db = databaseService;
        }

        internal async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (!(data.Configuration.Autorole is 0))
            {
                var targetRole = args.Guild.Roles.FirstOrDefault(r => r.Id == data.Configuration.Autorole);
                if (targetRole is null)
                {
                    await _logger.LogAsync(LogSeverity.Warning, LogSource.Service,
                        $"Guild {args.Guild.Name}'s Autorole is set to an ID of a role that no longer exists.");
                    return;
                }

                await args.User.AddRoleAsync(targetRole);
            }
        }
    }
}