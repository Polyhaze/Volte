namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Quote")]
    [Description("Quotes a user from a given message's ID.")]
    public async Task<ActionResult> QuoteAsync([Description("The ID of the message to quote.")]
        ulong messageId, [Description("The channel to get the message from. Defaults to the current channel.")]
        SocketTextChannel channel = null)
    {
        channel ??= Context.Channel;
        var m = await channel.GetMessageAsync(messageId);

        return m != null
            ? Ok(Context.CreateEmbedBuilder(new StringBuilder()
                    .AppendLine(m.Content)
                    .AppendLine()
                    .AppendLine(Format.Url("Jump!", m.GetJumpUrl())))
                .WithAuthor($"{m.Author}, in #{m.Channel}", m.Author.GetDisplayAvatarUrl())
                .WithFooter(m.Timestamp.Humanize()).Apply(e =>
                {
                    if (m.HasAttachments())
                        e.WithImageUrl(m.Attachments.First().Url);
                }))
            : BadRequest($"A message with that ID doesn't exist in {(channel.Id == Context.Channel.Id ? "this channel" : channel.Mention)}.");
    }
}