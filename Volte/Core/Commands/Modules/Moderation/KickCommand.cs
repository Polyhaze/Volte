using System.Threading.Tasks;
using Qmmands;
using Discord.WebSocket;
using Discord;
using Discord.Net;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("Usage: $kick {@user} [reason]")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireGuildModerator]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "Kicked by a Moderator.") {
            try {
                await Context.CreateEmbed($"You were kicked from **{Context.Guild.Name}** for **{reason}**.")
                    .SendTo(user);
            } catch (HttpException ignored) when (ignored.DiscordCode == 50007) {}
            

            await user.KickAsync(reason);
            await Context.CreateEmbed($"Successfully kicked **{user.Username}#{user.Discriminator}** from this server.")
                .SendTo(Context.Channel);
        }
    }
}