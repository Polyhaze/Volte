using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Ban")]
        [Summary("Bans the mentioned user.")]
        [Remarks("Usage: $ban {@user} [reason]")]
        [RequireGuildModerator]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "Banned by a Moderator.") {
            await Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.").SendTo(user);
            await Context.Guild.AddBanAsync(
                user, 0, reason);
            await Context.CreateEmbed($"Successfully banned **{user.Username}#{user.Discriminator}** from this guild.")
                .SendTo(Context.Channel);
        }
    }
}