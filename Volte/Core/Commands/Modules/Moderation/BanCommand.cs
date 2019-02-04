using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Commands.Preconditions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Ban")]
        [Summary("Bans the mentioned user.")]
        [Remarks("Usage: $ban {@user} [reason]")]
        [RequireGuildModerator]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "Banned by a Moderator.") {
            await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"You've been banned from {Context.Guild.Name} for **{reason}**."));
            await Context.Guild.AddBanAsync(
                user, 0, reason);
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"Successfully banned **{user.Username}#{user.Discriminator}** from this guild."));
        }
    }
}