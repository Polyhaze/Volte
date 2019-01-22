using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Moderation {
    public class BanCommand : SIVACommand {
        [Command("Ban")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "Banned by a Moderator.") {
            var config = ServerConfig.Get(Context.Guild);
            if (!UserUtils.HasRole(user, Context.Guild.Roles.FirstOrDefault(r => r.Id == config.ModRole))) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync("", false,
                CreateEmbed(Context, $"You've been banned from {Context.Guild.Name} for **{reason}**."));
            await Context.Guild.AddBanAsync(
                user, 0, $"Banned by {Context.User.Username}#{Context.User.Discriminator}");
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    $"Successfully banned **{user.Username}#{user.Discriminator}** from this server."));
        }
    }
}