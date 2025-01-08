using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("purge", "Purges the last x messages, or the last x messages by a given user.")]
    public async Task<RuntimeResult> PurgeAsync(
        [Summary("message_count", "The amount of messages to purge.")]
        int count, 
        [Summary("target_author", "If provided, will only delete messages by this user within 'message_count' messages.")]
        RestUser targetAuthor = null)
    {
        var messages = (await Context.Channel.GetMessagesAsync(count).FlattenAsync())
            .Where(x => targetAuthor is null || x.Author.Id == targetAuthor.Id)
            .ToList();
        
        try
        {
            await Context.Channel.Cast<SocketTextChannel>()
                .DeleteMessagesAsync(messages,
                    DiscordHelper.RequestOptions(opts =>
                        opts.AuditLogReason = $"Messages purged by {Context.User}."));
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest(
                $"Messages bulk deleted must be younger than 14 days. {Format.Code("This is a Discord restriction, not a Volte one.")}");
        }

        return Ok($"Successfully deleted {Format.Bold("message".ToQuantity(messages.Count))}.",
            () => ModService.OnModActionCompleteAsync(ModActionEventArgs
                .FromModule(this)
                .WithActionType(ModActionType.Purge)
                .WithCount(count)), 
            true);
    }
}