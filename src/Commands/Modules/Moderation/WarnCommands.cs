using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
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
        [Group("Warn", "Warns")]
        public sealed class WarnsModule : VolteModule
        {
            [Command]
            [Description("Warns the target user for the given reason.")]
            [Priority(100)]
            [Remarks("warn {Member} {String}")]
            public async Task<ActionResult> WarnAsync([CheckHierarchy] DiscordMember user, [Remainder] string reason)
            {
                await ModerationModule.WarnAsync(Context.Member, Context.GuildData, user, Db, Logger, reason);

                return Ok($"Successfully warned **{user}** for **{reason}**.",
                    _ => ModLogService.DoAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Warn)
                        .WithTarget(user)
                        .WithReason(reason))
                );
            }

            [Command("List", "L")]
            [Description("Shows all the warns for the given user.")]
            [Remarks("warn list {Member}")]
            public Task<ActionResult> WarnsAsync(DiscordMember user)
            {
                var warns = Context.GuildData.Extras.Warns.Where(x => x.User == user.Id).ToList();
                if (warns.IsEmpty()) return BadRequest("This user doesn't have any warnings.");
                else
                {
                    return Ok(new PaginatedMessageBuilder(Context)
                        .WithPages(warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**"))
                        .SplitPages(10));
                }
            }

            [Command("Clear", "C")]
            [Description("Clears the warnings for the given user.")]
            [Remarks("warn clear {Member}")]
            public async Task<ActionResult> ClearWarnsAsync(DiscordMember user)
            {
                var oldWarnList = Context.GuildData.Extras.Warns;
                var newWarnList = Context.GuildData.Extras.Warns.Where(x => x.User != user.Id).ToList();
                ModifyData(data =>
                {
                    data.Extras.Warns = newWarnList;
                    return data;
                });

                try
                {
                    await Context.CreateEmbed($"Your warns in **{Context.Guild.Name}** have been cleared. Hooray!")
                        .SendToAsync(user);
                }
                catch (UnauthorizedException e)
                {
                    Logger.Warn(LogSource.Volte,
                        $"encountered a 403 when trying to message {user}!", e);
                }

                return Ok($"Cleared **{oldWarnList.Count - newWarnList.Count}** warnings for **{user}**.", _ =>
                    ModLogService.DoAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.ClearWarns)
                        .WithTarget(user))
                );
            }
        }
    }
}