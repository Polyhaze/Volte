using System.Net;
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
    public sealed partial class ModerationModule : VolteModule
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
            catch (Discord.Net.HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                Logger.Debug(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!", e);
            }

            await user.BanAsync(daysToDelete, reason);
            return Ok($"Successfully banned **{user}** from this guild.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Ban)
                    .WithTarget(user)
                    .WithReason(reason))
                );
        }
    }
}