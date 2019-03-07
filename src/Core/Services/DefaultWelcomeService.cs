using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    public sealed class DefaultWelcomeService : IService
    {
        private readonly DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();

        internal async Task JoinAsync(SocketGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.WelcomeMessage.IsNullOrEmpty())
                return; //we don't want to send an empty join message
            var welcomeMessage = config.WelcomeOptions.WelcomeMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);
            var c = user.Guild.TextChannels.FirstOrDefault(channel =>
                channel.Id.Equals(config.WelcomeOptions.WelcomeChannel));

            if (!(c is null))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeOptions.WelcomeColorR, config.WelcomeOptions.WelcomeColorG,
                        config.WelcomeOptions.WelcomeColorB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await embed.SendTo(c);
            }
        }

        internal async Task LeaveAsync(SocketGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeOptions.LeavingMessage.IsNullOrEmpty()) return;
            var leavingMessage = config.WelcomeOptions.LeavingMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);
            var c = user.Guild.TextChannels.FirstOrDefault(channel =>
                channel.Id.Equals(config.WelcomeOptions.WelcomeChannel));
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

                await embed.SendTo(c);
            }
        }
    }
}