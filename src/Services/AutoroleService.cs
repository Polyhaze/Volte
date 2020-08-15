using System;
using System.Threading.Tasks;
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class AutoroleService : VolteEventService
    {
        private readonly LoggingService _logger;
        private readonly DatabaseService _db;

        public AutoroleService(LoggingService loggingService,
            DatabaseService databaseService)
        {
            _logger = loggingService;
            _db = databaseService;
        }

        public override Task DoAsync(EventArgs args)
            => ApplyRoleAsync(args.Cast<UserJoinedEventArgs>() ?? throw new InvalidOperationException($"AutoRole was triggered with a null event. Expected: {nameof(UserJoinedEventArgs)}, Received: {args.GetType().Name}"));

        private async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            var targetRole = args.Guild.GetRole(data.Configuration.Autorole);
            if (targetRole is not null)
            {
                await args.User.AddRoleAsync(targetRole);
                _logger.Debug(LogSource.Volte,
                    $"Applied role {targetRole.Name} to user {args.User} in guild {args.Guild.Name}.");
            }
        }
    }
}