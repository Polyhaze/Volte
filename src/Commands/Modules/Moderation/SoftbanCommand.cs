using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: |prefix|softban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers | GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> SoftBanAsync(SocketGuildUser user, int daysToDelete,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                Logger.Debug(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!", e);
            }

            await user.BanAsync(daysToDelete, reason);
            await Context.Guild.RemoveBanAsync(user);

            return Ok($"Successfully softbanned **{user.Username}#{user.Discriminator}**.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }
    }
}