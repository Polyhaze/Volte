using System;
using System.Threading.Tasks;
using Discord;
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
                    .WithContext(Context)
                    .WithActionType(ModActionType.IdBan)
                    .WithTargetId(user)
                    .WithReason(reason)
                    .WithModerator(Context.User)
                    .WithTime(DateTimeOffset.UtcNow)
                    .WithGuild(Context.Guild))
            );
        }
    }
}