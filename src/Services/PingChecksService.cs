using System;
using System.Threading.Tasks;
using Gommon;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class PingChecksService : VolteService
    {
        private readonly LoggingService _logger;

        public PingChecksService(LoggingService loggingService) 
            => _logger = loggingService;

        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.MassPingChecks &&
                !args.Context.User.IsAdmin(args.Context))
            {
                _logger.Debug(LogSource.Service,
                    "Received a message to check for ping threshold violations.");
                var content = args.Message.Content;
                if (content.ContainsIgnoreCase("@everyone") ||
                    content.ContainsIgnoreCase("@here") ||
                    args.Message.MentionedUsers.Count > 10)
                {
                    await args.Message.DeleteAsync();
                    _logger.Debug(LogSource.Service,
                        "Deleted a message for violating the ping threshold.");
                }
            }
        }
    }
}