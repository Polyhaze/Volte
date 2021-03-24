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
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target member for the given reason.")]
        public async Task<ActionResult> WarnAsync([CheckHierarchy, EnsureNotSelf, Description("The member to warn.")] SocketGuildUser member, [Remainder, Description("The reason for the warn.")] string reason)
        {
            await member.WarnAsync(Context, reason);

            return Ok($"Successfully warned **{member}** for **{reason}**.",
                _ => ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Warn)
                    .WithTarget(member)
                    .WithReason(reason))
            );
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given member.")]
        public Task<ActionResult> WarnsAsync([Remainder, Description("The member to list warns for.")] SocketGuildUser member)
        {
            var warns = Db.GetData(Context.Guild).Extras.Warns.Where(x => x.User == member.Id).Take(15);
            return Ok(new StringBuilder()
                .AppendLine(
                    "Showing the last 15 warnings; less if the member doesn't have 15 yet, or none if the member's record is clean.")
                .AppendLine()
                .AppendLine($"{warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").Join("\n")}")
                .ToString());
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given member.")]
        public async Task<ActionResult> ClearWarnsAsync([Remainder, EnsureNotSelf, Description("The member who you want to clear warns for.")] SocketGuildUser member)
        {
            var warnCount = Context.GuildData.Extras.Warns.RemoveAll(x => x.User == member.Id);
            Db.Save(Context.GuildData);

            try
            {
                await Context.CreateEmbed($"Your warns in **{Context.Guild.Name}** have been cleared. Hooray!")
                    .SendToAsync(member);
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!", e);
            }

            return Ok($"Cleared **{warnCount}** warnings for **{member}**.", _ =>
                ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.ClearWarns)
                    .WithTarget(member))
            );
        }
    }
}