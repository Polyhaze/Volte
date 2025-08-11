using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("purge", "Purges the last x messages, or the last x messages by a given user.")]
    public async Task<RuntimeResult> PurgeAsync(
        [Summary("message_count", "The amount of messages to purge.")]
        int count, 
        [Summary("target_author", "If provided, will only delete messages by this user within 'message_count' messages.")]
        string targetAuthor = null)
    {
        await DeferAsync(true);

        Func<IMessage, bool> filter = _ => true;

        if (targetAuthor != null)
        {
            if (!ulong.TryParse(targetAuthor, out var targetAuthorId))
                return BadRequest("target_author is not a valid Discord user ID.");

            filter = x => x.Author.Id == targetAuthorId;
        }


        var messages = (await Context.Channel.GetMessagesAsync(count).FlattenAsync())
            .Where(filter)
            .ToList();

        if (messages.Count == 0)
            return BadRequest("Cannot mass delete 0 messages.");
        
        try
        {
            await Context.Channel.Cast<SocketTextChannel>()
                .DeleteMessagesAsync(messages, new RequestOptions { AuditLogReason = $"Messages purged by {Context.User}." });
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest(
                $"Messages bulk deleted must be younger than 14 days. {Format.Code("This is a Discord restriction, not a Volte one.")}");
        }

        return Ok($"Successfully deleted {Format.Bold("message".ToQuantity(messages.Count))}.",
            ModAction(ModActionType.Purge).WithCount(messages.Count),
            ephemeral: true);
    }
}