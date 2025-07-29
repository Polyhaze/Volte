namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public sealed class GuildParser : VolteTypeParser<SocketGuild>
{
    public override ValueTask<TypeParserResult<SocketGuild>> ParseAsync(string value, VolteContext ctx) =>
        Parse(value, ctx);
        
    public static TypeParserResult<SocketGuild> Parse(string value, VolteContext ctx)
    {
        SocketGuild guild = default;

        var guilds = ctx.Client.Guilds;

        if (value.TryParse<ulong>(out var id))
            guild = guilds.FirstOrDefault(x => x.Id == id);

        if (guild is null)
        {
            var match = guilds.Where(x =>
                x.Name.EqualsIgnoreCase(value)).ToList();
            if (match.Count > 1)
                return Failure(
                    "Multiple guilds found with that name, try using its ID.");

            guild = match.FirstOrDefault();
        }

        return guild is null
            ? Failure("Guild not found.")
            : Success(guild);
    }
}