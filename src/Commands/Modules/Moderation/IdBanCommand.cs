using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("Usage: |prefix|idban {id} [reason]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> IdBanAsync(ulong user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.AddBanAsync(user, 0, reason);
            return Ok("Successfully banned that user from this guild.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }
    }
}