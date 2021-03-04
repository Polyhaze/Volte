using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target user for the given reason.")]
        [Remarks("warn {User} {String}")]
        [RequireGuildModerator]
        public async Task<ActionResult> WarnAsync([CheckHierarchy] SocketGuildUser user, [Remainder] string reason)
        {
            await user.WarnAsync(Context, reason);

            return Ok($"Successfully warned **{user}** for **{reason}**.",
                _ => ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Warn)
                    .WithTarget(user)
                    .WithReason(reason))
            );
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given user.")]
        [Remarks("warns {User}")]
        [RequireGuildModerator]
        public Task<ActionResult> WarnsAsync(SocketGuildUser user)
        {
            var warns = Db.GetData(Context.Guild).Extras.Warns.Where(x => x.User == user.Id).Take(15);
            return Ok(new StringBuilder()
                .AppendLine(
                    "Showing the last 15 warnings; less if the user doesn't have 15 yet, or none if the user's record is clean.")
                .AppendLine()
                .AppendLine($"{warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").Join("\n")}")
                .ToString());
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given user.")]
        [Remarks("clearwarns {User}")]
        [RequireGuildModerator]
        public async Task<ActionResult> ClearWarnsAsync(SocketGuildUser user)
        {
            var warnCount = Context.GuildData.Extras.Warns.RemoveAll(x => x.User == user.Id);
            Db.UpdateData(Context.GuildData);

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

            return Ok($"Cleared **{warnCount}** warnings for **{user}**.", _ =>
                ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.ClearWarns)
                    .WithTarget(user))
            );
        }
    }
}