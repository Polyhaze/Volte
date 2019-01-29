using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Helpers;
using Volte.Core.Files.Readers;
using System.Linq;
using Discord;

namespace Volte.Core.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Kick")]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "Kicked by a Moderator.") {
            var config = Db.GetConfig(Context.Guild);
            if (!UserUtils.HasRole(Context.User, config.ModRole)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync("", false,
                CreateEmbed(Context, $"You were kicked from **{Context.Guild.Name}** for **{reason}**."));

            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context,
                $"Successfully kicked {user.Username}#{user.Discriminator} from this server."));
        }
    }
}