using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target user for the given reason.")]
        [Remarks("warn {Member} {String}")]
        public async Task<ActionResult> WarnAsync([CheckHierarchy] SocketGuildUser user, [Remainder] string reason)
        {
            await WarnAsync(Context.User, Context.GuildData, user, Db, Logger, reason);

            return Ok($"Successfully warned **{user}** for **{reason}**.",
                _ => ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Warn)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given user.")]
        [Remarks("warns {Member}")]
        public Task<ActionResult> WarnsAsync(SocketGuildUser user)
        {
            var warns = Context.GuildData.Extras.Warns.Where(x => x.User == user.Id);
            if (warns.IsEmpty()) return BadRequest("This user doesn't have any warnings.");
            else
            {
                return Ok(new PaginatedMessageBuilder(Context)
                    .WithPages(warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**"))
                    .SplitPages(10));
            }
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given user.")]
        [Remarks("clearwarns {Member}")]
        public async Task<ActionResult> ClearWarnsAsync(SocketGuildUser user)
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
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
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