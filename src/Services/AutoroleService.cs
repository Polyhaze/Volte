using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Data.Models.EventArgs;
using Gommon;
using Volte.Data.Models;

namespace Volte.Services
{
    [Service("Autorole", "The main Service for automatically applying roles when a user joins any given guild.")]
    public sealed class AutoroleService
    {
        private LoggingService _logger;

        public AutoroleService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        internal async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            if (!(args.Data.Configuration.Autorole is 0))
            {
                var targetRole = args.Guild.Roles.FirstOrDefault(r => r.Id == args.Data.Configuration.Autorole);
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