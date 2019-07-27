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
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                "User joined a guild, let's check to see if we should send a welcome embed.");
            var welcomeMessage = data.Configuration.Welcome.WelcomeMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", args.Guild.Owner.Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", args.Guild.MemberCount.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Service, $"Sent a welcome embed to #{c.Name}.");
                return;
            }

            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                "WelcomeChannel config value was not set or resulted in an invalid channel; aborting.");
        }

        internal async Task LeaveAsync(UserLeftEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                "User left a guild, let's check to see if we should send a leaving embed.");
            var leavingMessage = data.Configuration.Welcome.LeavingMessage
                .Replace("{ServerName}", args.Guild.Name)
                .Replace("{UserName}", args.User.Username)
                .Replace("{UserMention}", args.User.Mention)
                .Replace("{OwnerMention}", args.Guild.Owner.Mention)
                .Replace("{UserTag}", args.User.Discriminator)
                .Replace("{MemberCount}", args.Guild.MemberCount.ToString())
                .Replace("{UserString}", args.User.ToString());
            var c = args.Guild.GetTextChannel(data.Configuration.Welcome.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(args.User.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
                await _logger.LogAsync(LogSeverity.Debug, LogSource.Service, $"Sent a leaving embed to #{c.Name}.");
                return;
            }

            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                "WelcomeChannel config value was not set or resulted in an invalid channel; aborting.");
        }
    }
}