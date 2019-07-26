using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    [Service("PingChecks", "The main Service used for checking if any given message contains mass mentions.")]
    public sealed class PingChecksService
    {
        private readonly LoggingService _logger;

        public PingChecksService(LoggingService loggingService) 
            => _logger = loggingService;

        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.MassPingChecks &&
                !args.Context.User.IsAdmin(args.Context.ServiceProvider))
            {
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                    "Received a message to check for ping threshold violations.");
                var content = args.Message.Content;
                if (content.ContainsIgnoreCase("@everyone") ||
                    content.ContainsIgnoreCase("@here") ||
                    args.Message.MentionedUsers.Count > 10)
                {
                    await args.Message.DeleteAsync();
                    await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                        "Deleted a message for violating the ping threshold.");
                }
            }
        }
    }
}