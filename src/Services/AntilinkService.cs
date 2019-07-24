using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Data.Models;
using Volte.Core.Data.Models.EventArgs;

namespace Volte.Services
{
    [Service("Antilink", "The main Service for checking links sent in chat.")]
    public sealed class AntilinkService
    {
        private readonly Regex _invitePattern = new Regex(@"discord(?:\.gg|\.io|\.me|app\.com\/invite)\/([\w\-]+)",
            RegexOptions.Compiled);

        private readonly LoggingService _logger;

        public AntilinkService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        internal async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Data.Configuration.Moderation.Antilink ||
                args.Context.User.IsAdmin(args.Context.ServiceProvider)) return;

            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                $"Checking a message in {args.Context.Guild.Name} for Discord invite URLs.");

            var matches = _invitePattern.Matches(args.Message.Content);
            if (!matches.Any()) return;

            await args.Message.DeleteAsync(new RequestOptions
                {AuditLogReason = "Deleted as it contained an invite link."});
            var m = await args.Context.CreateEmbed("Don't send invites here.").SendToAsync(args.Context.Channel);
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                $"Deleted a message in guild {args.Context.Guild.Name} for containing a Discord invite URL.");
            _ = Executor.ExecuteAfterDelayAsync(3000, async () => await m.DeleteAsync());
        }
    }
}