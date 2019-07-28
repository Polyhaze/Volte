using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Core.Models.Guild;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target user for the given reason.")]
        [Remarks("Usage: |prefix|warn {user} {reason}")]
        [RequireGuildModerator]
        public async Task<ActionResult> WarnAsync(SocketGuildUser user, [Remainder] string reason)
        {
            var data = Db.GetData(Context.Guild);
            data.Extras.Warns.Add(new Warn
            {
                User = user.Id,
                Reason = reason,
                Issuer = Context.User.Id,
                Date = DateTimeOffset.UtcNow
            });
            Db.UpdateData(data);

            try
            {
                await Context.CreateEmbed($"You've been warned in **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (HttpException ignored) when (ignored.DiscordCode == 50007)
            { }

            return Ok($"Successfully warned **{user}** for **{reason}**.",
                _ => ModLogService.DoAsync(ModActionEventArgs.New
                    .WithContext(Context)
                    .WithActionType(ModActionType.Warn)
                    .WithTargetUser(user)
                    .WithReason(reason)
                    .WithModerator(Context.User)
                    .WithTime(DateTimeOffset.UtcNow)
                    .WithGuild(Context.Guild))
            );
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given user.")]
        [Remarks("Usage: |prefix|warns {user}")]
        [RequireGuildModerator]
        public Task<ActionResult> WarnsAsync(SocketGuildUser user)
        {
            var warns = Db.GetData(Context.Guild).Extras.Warns.Where(x => x.User == user.Id).Take(10).ToList();
            return Ok("Showing the last 10 warnings, or less if the user doesn't have 10 yet." +
                      "\n" +
                      "\n" +
                      $"{warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").Join("\n")}");
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given user.")]
        [Remarks("Usage: |prefix|clearwarns {user}")]
        [RequireGuildModerator]
        public async Task<ActionResult> ClearWarnsAsync(SocketGuildUser user)
        {
            var data = Db.GetData(Context.Guild);
            var newWarnList = data.Extras.Warns.Where(x => x.User != user.Id).ToList();
            data.Extras.Warns = newWarnList;
            Db.UpdateData(data);

            try
            {
                await Context.CreateEmbed($"Your warns in **{Context.Guild.Name}** have been cleared. Hooray!")
                    .SendToAsync(user);
            }
            catch (HttpException ignored) when (ignored.DiscordCode == 50007)
            { }

            return Ok($"Cleared all warnings for **{user}**.", _ => 
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithContext(Context)
                    .WithActionType(ModActionType.ClearWarns)
                    .WithTargetUser(user)
                    .WithModerator(Context.User)
                    .WithTime(DateTimeOffset.UtcNow)
                    .WithGuild(Context.Guild)));
        }
    }
}