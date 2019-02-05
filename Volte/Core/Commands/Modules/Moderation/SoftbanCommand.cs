using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Softban")]
        [Summary("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: $softban {@user} [reason]")]
        [RequireGuildModerator]
        public async Task SoftBan(SocketGuildUser user, [Remainder]string reason = "Softbanned by a Moderator.") {
            await Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")
                .SendTo(user);
            await Context.Guild.AddBanAsync(
                user, 7, reason);
            await Context.Guild.RemoveBanAsync(user);
            await Context.CreateEmbed($"Successfully softbanned **{user.Username}#{user.Discriminator}**.")
                .SendTo(Context.Channel);
        }
    }
}