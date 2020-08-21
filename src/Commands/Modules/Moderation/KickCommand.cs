using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("kick {Member} [String]")]
        [RequireBotGuildPermission(Permissions.KickMembers)]
        public async Task<ActionResult> KickAsync([CheckHierarchy] DiscordMember user,
            [Remainder] string reason = "Kicked by a Moderator.")
        {
            if (!await user.TrySendMessageAsync(
                embed: Context.CreateEmbed($"You've been kicked from **{Context.Guild.Name}** for **{reason}**.")))
            {
                Logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {user}!");
            }

            await user.RemoveAsync(reason);

            return Ok($"Successfully kicked **{user.Username}#{user.Discriminator}** from this guild.", m =>
                ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Kick)
                    .WithTarget(user)
                    .WithReason(reason))
                );
        }
    }
}