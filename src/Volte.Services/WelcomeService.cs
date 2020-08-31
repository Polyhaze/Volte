using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gommon;
using Volte.Core.Entities;

namespace Volte.Services
{
    public sealed class WelcomeService : VolteEventService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public WelcomeService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }
        
        public override Task DoAsync(EventArgs args)
        {
            return args switch
            {
                GuildMemberAddEventArgs joined => JoinAsync(joined),
                GuildMemberRemoveEventArgs left => LeaveAsync(left),
                _ => Task.CompletedTask
            };
        }

        internal async Task JoinAsync(GuildMemberAddEventArgs args)
        {
            var data = _db.GetData(args.Guild);

            if (!data.Configuration.Welcome.WelcomeDmMessage.IsNullOrEmpty())
                await JoinDmAsync(args);
            if (data.Configuration.Welcome.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message

            _logger.Debug(LogSource.Volte,
                "User joined a guild, let's check to see if we should send a welcome embed.");
            var welcomeMessage = data.Configuration.Welcome.FormatWelcomeMessage(args.Member);
            var c = args.Guild.GetChannel(data.Configuration.Welcome.WelcomeChannel);

            if (c is not null)
            {
                await new DiscordEmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(welcomeMessage)
                    .WithThumbnail(args.Member.AvatarUrl)
                    .WithCurrentTimestamp()
                    .SendToAsync(c);

                _logger.Debug(LogSource.Volte, $"Sent a welcome embed to #{c.Name}.");
                return;
            }

            _logger.Debug(LogSource.Volte,
                "WelcomeChannel config value resulted in an invalid/nonexistent channel; aborting.");
        }

        internal Task JoinDmAsync(GuildMemberAddEventArgs args)
            => args.Member.TrySendMessageAsync(_db.GetData(args.Guild).Configuration.Welcome.FormatDmMessage(args.Member));

        internal async Task LeaveAsync(GuildMemberRemoveEventArgs args)
        {
            var data = _db.GetData(args.Guild);
            if (data.Configuration.Welcome.LeavingMessage.IsNullOrEmpty()) return;
            _logger.Debug(LogSource.Volte,
                "User left a guild, let's check to see if we should send a leaving embed.");
            var leavingMessage = data.Configuration.Welcome.FormatLeavingMessage(args.Member);
            var c = args.Guild.GetChannel(data.Configuration.Welcome.WelcomeChannel);
            if (c is not null)
            {
                await new DiscordEmbedBuilder()
                    .WithColor(data.Configuration.Welcome.WelcomeColor)
                    .WithDescription(leavingMessage)
                    .WithThumbnail(args.Member.GetAvatarUrl(ImageFormat.Auto, 256))
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