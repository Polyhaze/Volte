namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public sealed class ChannelParser : VolteTypeParser<SocketTextChannel>
{
    public override ValueTask<TypeParserResult<SocketTextChannel>> ParseAsync(string value, VolteContext ctx) =>
        Parse(value, ctx);
        
    public static TypeParserResult<SocketTextChannel> Parse(string value, VolteContext ctx)
    {
        SocketTextChannel channel = default;

        if (value.TryParse<ulong>(out var id) || MentionUtils.TryParseChannel(value, out id))
            channel = ctx.Client.GetChannel(id).Cast<SocketTextChannel>();

        if (channel is null)
        {
            var match = ctx.Guild.TextChannels.Where(x => x.Name.EqualsIgnoreCase(value))
                .ToList();
            if (match.Count > 1)
                return Failure(
                    "Multiple channels found. Try mentioning the channel or using its ID.");
            channel = match.First();
        }

        return channel is null
            ? Failure("Channel not found.")
            : Success(channel);
    }
}