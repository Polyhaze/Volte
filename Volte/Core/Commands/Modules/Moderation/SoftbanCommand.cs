using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Softban")]
        [Summary("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: $softban {@user} [reason]")]
        [RequireGuildModerator]
        public async Task SoftBan(SocketGuildUser user, [Remainder]string reason = "Softbanned by a Moderator.") {
            await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"You've been softbanned from **{Context.Guild.Name}** for **{reason}**."));
            await Context.Guild.AddBanAsync(
                user, 7, reason);
            await Context.Guild.RemoveBanAsync(user);
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"Successfully softbanned **{user.Username}#{user.Discriminator}**."));
        }
    }
}