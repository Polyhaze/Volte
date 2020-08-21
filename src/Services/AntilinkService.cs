using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class AntilinkService : VolteEventService
    {
        private readonly Regex _invitePattern =
            new Regex(@"discord(?:\.gg|\.io|\.me|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);

        private readonly LoggingService _logger;

        public AntilinkService(LoggingService loggingService)
            => _logger = loggingService;

        public override Task DoAsync(EventArgs args) 
            => CheckMessageAsync(args.Cast<MessageReceivedEventArgs>() ?? throw new InvalidOperationException($"AntiLink was triggered with a null event. Expected: {nameof(MessageReceivedEventArgs)}, Received: {args.GetType().Name}"));
        
        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Data.Configuration.Moderation.Antilink ||
                args.Context.Member.IsAdmin(args.Context)) return;

            _logger.Debug(LogSource.Volte,
                $"Checking a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for Discord invite URLs.");

            var matches = _invitePattern.Matches(args.Message.Content);
            if (matches.IsEmpty())
            {
                _logger.Debug(LogSource.Volte,
                    $"Message checked in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) did not contain any detectable invites; aborted.");
                return;
            }

            _ = await args.Message.TryDeleteAsync("Deleted as it contained an invite link.");
            var m = await args.Context.CreateEmbed($"{args.Message.Author.Mention}, Don't send invites here.").SendToAsync(args.Context.Channel);
            _logger.Debug(LogSource.Volte,
                $"Deleted a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for containing a Discord invite URL.");
            _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), () => m.TryDeleteAsync());
        }
    }
}