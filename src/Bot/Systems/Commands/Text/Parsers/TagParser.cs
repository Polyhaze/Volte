using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public sealed class TagParser : VolteTypeParser<TagV2>
{
    public override ValueTask<TypeParserResult<TagV2>> ParseAsync(string value, VolteContext ctx)
    {
        if (ctx.GuildData.GetTagByNameOrAlias(value).TryGet(out var tag))
            return Success(tag);

        return Failure($"The tag **{value}** doesn't exist in this guild. " +
                       $"Try using the `{ctx.FormatUsageFor("Tags List")}` command to see all tags in this guild.");
    }
}