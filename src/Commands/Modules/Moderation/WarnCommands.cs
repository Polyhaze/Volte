using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target user for the given reason.")]
        public async Task<ActionResult> WarnAsync([CheckHierarchy, EnsureNotSelf, Description("The member to warn.")] SocketGuildUser user, [Remainder, Description("The reason for the warn.")] string reason)
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
        public Task<ActionResult> WarnsAsync([Remainder, Description("The member to list warns for.")] SocketGuildUser user)
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
        public async Task<ActionResult> ClearWarnsAsync([Remainder, EnsureNotSelf, Description("The user who you want to clear warns for.")] SocketGuildUser user)
        {
            var warnCount = Context.GuildData.Extras.Warns.RemoveAll(x => x.User == user.Id);
            Db.Save(Context.GuildData);

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