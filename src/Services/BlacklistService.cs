using System;
using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class BlacklistService : VolteEventService
    {
        private readonly LoggingService _logger;

        public BlacklistService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        public override Task DoAsync(EventArgs args)
        {
            return CheckMessageAsync(args.Cast<MessageReceivedEventArgs>());
        }

        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            _logger.Debug(LogSource.Volte, "Checking a message for blacklisted words.");
            foreach (var word in args.Data.Configuration.Moderation.Blacklist)
                if (args.Message.Content.ContainsIgnoreCase(word))
                {
                    await args.Message.DeleteAsync();
                    _logger.Debug(LogSource.Volte, $"Deleted a message for containing {word}.");
                    return;
                }
        }
    }
}