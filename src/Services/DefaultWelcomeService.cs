using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Services
{
    public sealed class DefaultWelcomeService : IService
    {
        private readonly DatabaseService _db;

        public DefaultWelcomeService(DatabaseService databaseService) => _db = databaseService;

        internal async Task JoinAsync(IGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = config.WelcomeOptions.WelcomeMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", (await user.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", user.Discriminator);
            var c = await user.Guild.GetTextChannelAsync(config.WelcomeOptions.WelcomeChannel);

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeOptions.WelcomeColorR, config.WelcomeOptions.WelcomeColorG,
                        config.WelcomeOptions.WelcomeColorB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }

        internal async Task LeaveAsync(IGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = config.WelcomeOptions.LeavingMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", (await user.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", user.Discriminator);
            var c = await user.Guild.GetTextChannelAsync(config.WelcomeOptions.WelcomeChannel);
            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeOptions.WelcomeColorR,
                        config.WelcomeOptions.WelcomeColorG,
                        config.WelcomeOptions.WelcomeColorB)
                    )
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendToAsync(c);
            }
        }
    }
}