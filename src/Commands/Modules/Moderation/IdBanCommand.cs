using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("idban {Ulong} [String]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> IdBanAsync(ulong user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.BanMemberAsync(user, 0, reason);
            return Ok("Successfully banned that user from this guild.", async m => 
                await ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }
    }
}