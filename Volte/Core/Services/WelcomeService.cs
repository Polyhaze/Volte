using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    internal class WelcomeService : IService
    {
        private readonly DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();

        internal async Task JoinAsync(SocketGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.WelcomeMessage.IsNullOrEmpty()) return; //we don't want to send an empty join message
            var welcomeMessage = config.WelcomeMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);

            if (user.Guild.TextChannels.Any(c => c.Id == config.WelcomeChannel)) //if the channel even exists
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeColorR, config.WelcomeColorG, config.WelcomeColorB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await user.Guild.GetTextChannel(config.WelcomeChannel)
                    .SendMessageAsync(string.Empty, false, embed.Build());
            }
        }

        internal async Task LeaveAsync(SocketGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            if (config.LeavingMessage.IsNullOrEmpty()) return; //we don't want to send an empty leaving message
            var leavingMessage = config.LeavingMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{UserMention}", user.Mention)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);

            if (user.Guild.TextChannels.Any(c => c.Id == config.WelcomeChannel)) //if the channel even exists
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeColorR, config.WelcomeColorG, config.WelcomeColorB))
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                await VolteBot.Client.GetGuild(user.Guild.Id).GetTextChannel(config.WelcomeChannel)
                    .SendMessageAsync(string.Empty, false, embed.Build());
            }
        }
    }
}