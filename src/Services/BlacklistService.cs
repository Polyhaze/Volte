using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    [Service("Blacklist", "The main Service for checking messages for blacklisted words/phrases in user's messages.")]
    public sealed class BlacklistService
    {
        private readonly LoggingService _logger;

        public BlacklistService(LoggingService loggingService) 
            => _logger = loggingService;

        internal async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte, "Checking a message for blacklisted words.");
            foreach (var word in args.Data.Configuration.Moderation.Blacklist)
                if (args.Message.Content.ContainsIgnoreCase(word))
                {
                    await args.Message.DeleteAsync();
                    await _logger.LogAsync(LogSeverity.Debug, LogSource.Volte,
                        $"Deleted a message for containing {word}.");
                    return;
                }
        }
    }
}