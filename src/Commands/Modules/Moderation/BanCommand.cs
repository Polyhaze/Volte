using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("Usage: |prefix|ban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BanAsync(SocketGuildUser user, int daysToDelete,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (Discord.Net.HttpException ignored) when (ignored.DiscordCode == 50007) { }

            await user.BanAsync(daysToDelete, reason);
            return Ok($"Successfully banned **{user.Username}#{user.Discriminator}** from this guild.", _ =>
                ModLogService.DoAsync(new ModActionEventArgs(Context, ModActionType.Ban, user,
                    reason)));
        }
    }
}