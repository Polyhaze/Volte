using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
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
            => CheckMessageAsync(args.Cast<MessageReceivedEventArgs>());


        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Data.Configuration.Moderation.Antilink ||
                args.Context.User.IsAdmin(args.Context.ServiceProvider)) return;

            _logger.Debug(LogSource.Volte,
                $"Checking a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for Discord invite URLs.");

            var matches = _invitePattern.Matches(args.Message.Content);
            if (!matches.Any())
            {
                _logger.Debug(LogSource.Volte,
                    $"Message checked in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) did not contain any detectable invites; aborted.");
                return;
            }

            await args.Message.DeleteAsync(new RequestOptions
                {AuditLogReason = "Deleted as it contained an invite link."});
            var m = await args.Context.CreateEmbed("Don't send invites here.").SendToAsync(args.Context.Channel);
            _logger.Debug(LogSource.Volte,
                $"Deleted a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for containing a Discord invite URL.");
            _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), () => m.DeleteAsync());
        }
    }
}