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
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("kick {User} [String]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> KickAsync([CheckHierarchy] SocketGuildUser user,
            [Remainder] string reason = "Kicked by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been kicked from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }
            
            try
            {
                await user.KickAsync(reason);
                return Ok($"Successfully kicked **{user.Username}#{user.Discriminator}** from this guild.", _ =>
                    ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                        .WithDefaultsFromContext(Context)
                        .WithActionType(ModActionType.Kick)
                        .WithTarget(user)
                        .WithReason(reason))
                );
            }
            catch
            {
                return BadRequest("An error occurred kicking that user. Do I have permission; or are they higher than me in the role list?");
            }
        }
    }
}