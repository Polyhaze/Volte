using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class WelcomeService : VolteService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public WelcomeService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }

        internal async Task JoinAsync(UserJoinedEventArgs args)
        {
            var data = _db.GetData(args.Guild);

            if (data.Configuration.Welcome.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            if (!data.Configuration.Welcome.WelcomeDmMessage.IsNullOrEmpty())
                _ = await args.User.TrySendMessageAsync(data.Configuration.Welcome.FormatDmMessage(args.User));

            _logger.Debug(LogSource.Volte,
                "User joined a guild, let's check to see if we should send a welcome embed.");
            var welcomeMessage = data.Configuration.Welcome.FormatWelcomeMessage(args.User);
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);

            if (!(c is null))
            {
                await new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .SendToAsync(c);

                _logger.Debug(LogSource.Volte, $"Sent a welcome embed to #{c.Name}.");
                return;
            }

            _logger.Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
        }

        internal async Task LeaveAsync(UserLeftEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            _logger.Debug(LogSource.Volte,
                "User left a guild, let's check to see if we should send a leaving embed.");
            var leavingMessage = data.Configuration.Welcome.FormatLeavingMessage(args.User);
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);
            if (!(c is null))
            {
                await new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .SendToAsync(c);
                _logger.Debug(LogSource.Volte, $"Sent a leaving embed to #{c.Name}.");
                return;
            }

            _logger.Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
        }
    }
}