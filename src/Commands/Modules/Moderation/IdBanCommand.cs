using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Attributes;
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
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> IdBanAsync(ulong user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.AddBanAsync(user, 0, reason);
            return Ok("Successfully banned that user from this guild.", async _ => 
                await ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }
    }
}