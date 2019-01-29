using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Ban")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "Banned by a Moderator.") {
            var config = Db.GetConfig(Context.Guild);
            if (!UserUtils.HasRole(Context.User, config.ModRole)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync("", false,
                CreateEmbed(Context, $"You've been banned from {Context.Guild.Name} for **{reason}**."));
            await Context.Guild.AddBanAsync(
                user, 0, $"Banned by {Context.User.Username}#{Context.User.Discriminator}");
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    $"Successfully banned **{user.Username}#{user.Discriminator}** from this server."));
        }
    }
}