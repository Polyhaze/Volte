using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Automation
{
    public class Welcome
    {
        public static async Task Join(SocketGuildUser user)
        {
            var config = ServerConfig.Get(user.Guild);

            var welcomeMessage = config.WelcomeMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);
            
            if (user.Guild.TextChannels.Any(c => c.Id == config.WelcomeChannel))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeColourR, config.WelcomeColourG, config.WelcomeColourB))
                    .WithDescription(welcomeMessage)
                    .WithThumbnailUrl(user.Guild.IconUrl)
                    .WithCurrentTimestamp();

                await SIVA.GetInstance.GetGuild(user.Guild.Id).GetTextChannel(config.WelcomeChannel)
                    .SendMessageAsync("", false, embed.Build());
            }
        }

        public static async Task Leave(SocketGuildUser user)
        {
            var config = ServerConfig.Get(user.Guild);
            
            var leavingMessage = config.LeavingMessage
                .Replace("{ServerName}", user.Guild.Name)
                .Replace("{UserName}", user.Username)
                .Replace("{OwnerMention}", user.Guild.Owner.Mention)
                .Replace("{UserTag}", user.Discriminator);
            
            if (user.Guild.TextChannels.Any(c => c.Id == config.WelcomeChannel))
            {
                var embed = new EmbedBuilder()
                    .WithColor(new Color(config.WelcomeColourR, config.WelcomeColourG, config.WelcomeColourB))
                    .WithDescription(leavingMessage)
                    .WithThumbnailUrl(user.Guild.IconUrl)
                    .WithCurrentTimestamp();

                await SIVA.GetInstance.GetGuild(user.Guild.Id).GetTextChannel(config.WelcomeChannel)
                    .SendMessageAsync("", false, embed.Build());
            }
        }
    }
}