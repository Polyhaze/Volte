using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class TagParser : VolteTypeParser<Tag>
    {
        public override ValueTask<TypeParserResult<Tag>> ParseAsync(string value, VolteContext ctx)
        {
            if (ctx.GuildData.Extras.Tags.AnyGet(x => x.Name.EqualsIgnoreCase(value), out var tag))
                return Success(tag);

            return Failure($"The tag **{value}** doesn't exist in this guild. " +
                                                    $"Try using the `{CommandHelper.FormatUsage(ctx, ctx.Services.Get<CommandService>().GetCommand("Tags List"))}` command to see all tags in this guild.");
        }
    }
}