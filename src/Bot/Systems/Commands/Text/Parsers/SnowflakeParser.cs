namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public class SnowflakeParser : TypeParser<Snowflake>
{
    public override ValueTask<TypeParserResult<Snowflake>> ParseAsync(Parameter param, string value, CommandContext ctx)
    {
        return Snowflake.TryParse(value, out var snowflake)
            ? TypeParserResult<Snowflake>.Successful(snowflake)
            : TypeParserResult<Snowflake>.Failed($"Value passed for {param.Name} must be a Discord ID (aka [Snowflake](https://discord.com/developers/docs/reference#snowflakes)).");
    }
}