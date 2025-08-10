using Volte.Systems.Moderation;

namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Delete")]
    [Description(
        "Deletes a message in the current channel by its ID. Creates an audit log entry for abuse prevention.")]
    public async Task<ActionResult> DeleteAsync(
        [Description("The ID of the message to delete. Must be in the current channel.")]
        ulong messageId)
    {
        var target = await Context.Channel.GetMessageAsync(messageId);
        if (target is null)
            return BadRequest("That message doesn't exist in this channel.");

        await target.TryDeleteAsync($"Message deleted by Moderator {Context.User}.");

        return None(async () =>
        {
            await Interactive.ReplyAndDeleteAsync(Context, 
                $"{Emojis.BallotBoxWithCheck} Deleted that message.", 
                timeout: 3.Seconds());
            await Context.Message.TryDeleteAsync();
            
            ModerationService.CallEvent(Context.ModAction
                .WithActionType(ModActionType.Delete)
                .WithTarget(messageId)
            );
        });
    }
}