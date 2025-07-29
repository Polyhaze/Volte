namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public sealed class UnixParser : VolteTypeParser<Dictionary<string, string>>
{
    public override ValueTask<TypeParserResult<Dictionary<string, string>>> ParseAsync(string value, VolteContext _) 
        => UnixHelper.TryParseNamedArguments(value, out var result)
            ? Success(result.Parsed)
            : Failure(result.Error.Message);
}