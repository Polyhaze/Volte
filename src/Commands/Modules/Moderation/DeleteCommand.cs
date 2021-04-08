using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Delete")]
        [Description(
            "Deletes a message in the current channel by its ID. Creates an audit log entry for abuse prevention.")]
        [RequireBotChannelPermission(ChannelPermission.ManageMessages)]
        public async Task<ActionResult> DeleteAsync(
            [Description("The ID of the message to delete. Must be in the current channel.")]
            ulong messageId)
        {
            var target = await Context.Channel.GetMessageAsync(messageId);
            if (target is null)
                return BadRequest("That message doesn't exist in this channel.");

            await target.TryDeleteAsync($"Message deleted by Moderator {Context.User}.");

            return Ok($"{DiscordHelper.BallotBoxWithCheck} Deleted that message.", async m =>
            {
                //await Interactive.ReplyAndDeleteAsync(Context,)
                _ = Executor.ExecuteAfterDelayAsync(3.Seconds(), async () =>
                {
                    await Context.Message.TryDeleteAsync();
                    await m.DeleteAsync();
                });

                await ModerationService.OnModActionCompleteAsync(ModActionEventArgs.New
                    .WithDefaultsFromContext(Context)
                    .WithActionType(ModActionType.Delete)
                    .WithTarget(messageId)
                );
            });
        }
    }
}