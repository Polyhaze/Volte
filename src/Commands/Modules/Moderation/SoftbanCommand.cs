using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
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
        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: |prefix|softban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers | GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> SoftBanAsync(SocketGuildUser user, int daysToDelete,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (Discord.Net.HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                Logger.Log(LogSeverity.Debug, LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!", e);
            }

            await user.BanAsync(daysToDelete, reason);
            await Context.Guild.RemoveBanAsync(user);

            return Ok($"Successfully softbanned **{user.Username}#{user.Discriminator}**.", _ =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithContext(Context)
                    .WithTargetUser(user)
                    .WithReason(reason)
                    .WithModerator(Context.User)
                    .WithTime(DateTimeOffset.UtcNow)
                    .WithGuild(Context.Guild)));
        }
    }
}