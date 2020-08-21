using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("ban {Member} [String]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> BanAsync([CheckHierarchy] DiscordMember user,
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