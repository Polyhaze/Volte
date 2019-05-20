using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Volte.Data.Models.Guild;
using Volte.Extensions;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Warn", "W")]
        [Description("Warns the target user for the given reason.")]
        [Remarks("Usage: |prefix|warn {user} {reason}")]
        [RequireGuildModerator]
        public async Task WarnAsync(SocketGuildUser user, [Remainder] string reason)
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
            catch (HttpException ignored) when (ignored.DiscordCode == 50007) { }

            await Context.CreateEmbed($"Successfully warned **{user}** for **{reason}**.").SendToAsync(Context.Channel);

            await ModLogService.OnModActionCompleteAsync(new ModActionEventArgs(Context, ModActionType.Warn, user, reason));
        }

        [Command("Warns", "Ws")]
        [Description("Shows all the warns for the given user.")]
        [Remarks("Usage: |prefix|warns {user}")]
        [RequireGuildModerator]
        public async Task WarnsAsync(SocketGuildUser user)
        {
            var data = Db.GetData(Context.Guild);
            var warns = data.Extras.Warns.Where(x => x.User == user.Id).Take(10);
            var desc = "Showing the last 10 warnings, or less if the user doesn't have 10 yet." +
                       "\n" +
                       "\n" +
                       $"{warns.Select(x => $"**{x.Reason}**, on **{x.Date.FormatDate()}**").Join("\n")}";
            await Context.CreateEmbed(desc).SendToAsync(Context.Channel);
        }

        [Command("ClearWarns", "Cw")]
        [Description("Clears the warnings for the given user.")]
        [Remarks("Usage: |prefix|clearwarns {user}")]
        [RequireGuildModerator]
        public async Task ClearWarnsAsync(SocketGuildUser user)
        {
            var data = Db.GetData(Context.Guild);
            var newWarnList = data.Extras.Warns.Where(x => x.User != user.Id).ToList();
            data.Extras.Warns = newWarnList;
            Db.UpdateData(data);

            await Context.CreateEmbed($"Cleared all warnings for **{user}**.").SendToAsync(Context.Channel);
            await ModLogService.OnModActionCompleteAsync(new ModActionEventArgs(Context, ModActionType.ClearWarns, user, null));
        }

    }
}