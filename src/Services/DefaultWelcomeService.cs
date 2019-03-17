using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("DefaultWelcome", "The main Service that handles default welcome/leaving functionality when no WelcomeApiKey is set in the bot config.")]
    public sealed class DefaultWelcomeService
    {
        private readonly DatabaseService _db;

        public DefaultWelcomeService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task JoinAsync(DiscordMember user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = config.WelcomeOptions.WelcomeMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator)
                .Replace("{MemberCount}", user.Guild.MemberCount.ToString())
                .Replace("{UserString}", user.ToHumanReadable());
            var c = user.Guild.GetChannel(config.WelcomeOptions.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor(config.WelcomeOptions.WelcomeColorR,
                        config.WelcomeOptions.WelcomeColorG,
                        config.WelcomeOptions.WelcomeColorB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(user.AvatarUrl)
                    .WithTimestamp(user.JoinedAt.UtcDateTime);

                await embed.SendToAsync(c);
            }
        }

        internal async Task LeaveAsync(DiscordMember user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = config.WelcomeOptions.LeavingMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator)
                .Replace("{MemberCount}", user.Guild.MemberCount.ToString())
                .Replace("{UserString}", user.ToHumanReadable());
            var c = user.Guild.GetChannel(config.WelcomeOptions.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor(
                        config.WelcomeOptions.WelcomeColorR,
                        config.WelcomeOptions.WelcomeColorG,
                        config.WelcomeOptions.WelcomeColorB)
                    )
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(user.AvatarUrl)
                    .WithTimestamp(user.JoinedAt.UtcDateTime);

                await embed.SendToAsync(c);
            }
        }
    }
}