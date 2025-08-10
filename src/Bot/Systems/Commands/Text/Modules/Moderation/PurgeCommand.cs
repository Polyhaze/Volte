namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Purge", "Clear", "Clean")]
    [Description("Purges the last x messages, or the last x messages by a given user.")]
    public async Task<ActionResult> PurgeAsync([Description("The amount of messages to purge.")]
        int count, [Description("If provided, will only delete messages by this user within `count` messages.")]
        RestUser targetAuthor = null)
    {
        //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
        //lets you theoretically use 0 to delete only the invocation message, for testing or something.
        var messages = (await Context.Channel.GetMessagesAsync(count + 1).FlattenAsync())
            .Where(x => targetAuthor is null || x.Author.Id == targetAuthor.Id)
            .ToHashSet();
        
        try
        {
            await Context.Channel.DeleteMessagesAsync(messages, new RequestOptions { AuditLogReason = $"Messages purged by {Context.User}." });
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest(
                $"Messages bulk deleted must be younger than 14 days. {Format.Code("This is a Discord restriction, not a Volte one.")}");
        }

        //-1 to show that the correct amount of messages were deleted.
        return None(async () =>
        {
            await Interactive.ReplyAndDeleteAsync(Context, string.Empty,
                embed: Context.CreateEmbed($"Successfully deleted {Format.Bold("message".ToQuantity(messages.Count - 1))}."),
                timeout: 3.Seconds());
            Moderation.ModerationService.CallEvent(Context.ModAction
                .WithActionType(ModActionType.Purge)
                .WithCount(messages.Count - 1)
            );
        }, false);
    }
}