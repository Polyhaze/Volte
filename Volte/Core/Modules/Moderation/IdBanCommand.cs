using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("IdBan")]
        [Summary("Bans a user based on their ID.")]
        [Remarks("Usage: $idban {id} [reason]")]
        public async Task IdBan(ulong user, [Remainder] string reason = "Banned by a Moderator.") {
            if (!UserUtils.IsModerator(Context)) {
                await Context.ReactFailure();
                return;
            }

            await Context.Guild.AddBanAsync(user, 0, reason);
            await Reply(Context.Channel, CreateEmbed(Context, "Successfully banned that user from this guild."));
        }
    }
}