using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Helpers;
using Volte.Core.Data;
using System.Linq;
using Discord;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Kick")]
        [Summary("Kicks the given user.")]
        [Remarks("Usage: $kick {@user} [reason]")]
        [RequireGuildModerator]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "Kicked by a Moderator.") {
            await Context.CreateEmbed($"You were kicked from **{Context.Guild.Name}** for **{reason}**.").SendTo(user);

            await user.KickAsync(reason);
            await Context.CreateEmbed($"Successfully kicked **{user.Username}#{user.Discriminator}** from this server.")
                .SendTo(Context.Channel);
        }
    }
}