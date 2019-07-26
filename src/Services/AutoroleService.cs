using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    [Service("Autorole", "The main Service for automatically applying roles when a user joins any given guild.")]
    public sealed class AutoroleService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;

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
                    await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                        $"Guild {args.Guild.Name}'s Autorole is set to an ID of a role that no longer exists.");
                    return;
                }

                await args.User.AddRoleAsync(targetRole);
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                    $"Applied role {targetRole.Name} to user {args.User} in guild {args.Guild.Name}.");
            }
        }
    }
}