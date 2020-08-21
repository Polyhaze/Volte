using System;
using System.Threading.Tasks;
using DSharpPlus;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Delete")]
        [Description("Deletes a message in the current channel by its ID. Creates an audit log entry for abuse prevention.")]
        [Remarks("delete {Ulong}")]
        [RequireBotChannelPermission(Permissions.ManageMessages)]
        public async Task<ActionResult> DeleteAsync(ulong messageId)
        {
            var target = await Context.Channel.GetMessageAsync(messageId);
            if (target is null)
                return BadRequest("That message doesn't exist in this channel.");

            await target.TryDeleteAsync($"Message deleted by Moderator {Context.Member}.");

            return Ok($"{EmojiHelper.BallotBoxWithCheck} Deleted that message.", async m =>
            {
                _ = Executor.ExecuteAfterDelayAsync(TimeSpan.FromSeconds(3), async () =>
                {
                    await Context.Message.TryDeleteAsync();
                    await m.DeleteAsync();
                });

                await ModLogService.DoAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Delete)
                    .WithTarget(messageId)
                );
            });
        }
    }
}