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
        public async Task<ActionResult> BanAsync([CheckHierarchy] SocketGuildUser user, int daysToDelete,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
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

        [Command("Ban")]
        [Description("Bans the mentioned user, deleting the past 7 days of messages")]
        [Remarks("Usage: |prefix|ban {@user} [reason]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BanAsync([CheckHierarchy] SocketGuildUser user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            await user.BanAsync(7, reason);
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