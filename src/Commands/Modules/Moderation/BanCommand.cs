using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("ban {User} [String]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BanAsync([CheckHierarchy] SocketGuildUser user,
            [Remainder] string reason = "Banned by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            try
            {
                await user.BanAsync(7, reason);
                return Ok($"Successfully banned **{user}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Ban)
                        .WithTarget(user)
                        .WithReason(reason))
                );
            }
            catch
            {
                return BadRequest("An error occurred banning that user. Do I have permission; or are they higher than me in the role list?");
            }
        }
    }
}