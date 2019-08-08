using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Delete")]
        [Description("Deletes a message by its ID. Creates an audit log entry for abuse prevention.")]
        [Remarks("Usage: |prefix|delete {messageId}")]
        [RequireGuildModerator]
        [RequireBotChannelPermission(ChannelPermission.ManageMessages)]
        public async Task<ActionResult> DeleteAsync(ulong messageId)
        {
            var target = await Context.Channel.GetMessageAsync(messageId);
            if (target is null)
                return BadRequest("That message doesn't exist in this channel.");

            await target.DeleteAsync(new RequestOptions
            {
                AuditLogReason = $"Message deleted by Moderator {Context.User}."
            });

            return Ok($"{EmojiService.BallotBoxWithCheck} Deleted that message.", async m =>
            {
                _ = Executor.ExecuteAfterDelayAsync(3000, async () =>
                {
                    await Context.Message.DeleteAsync();
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