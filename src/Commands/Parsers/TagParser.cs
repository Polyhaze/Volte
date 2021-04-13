using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands
{
    [VolteTypeParser]
    public sealed class TagParser : TypeParser<Tag>
    {
        public override ValueTask<TypeParserResult<Tag>> ParseAsync(Parameter _, string value,
            CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.GuildData.Extras.Tags.AnyGet(x => x.Name.EqualsIgnoreCase(value), out var tag))
                return TypeParserResult<Tag>.Successful(tag);

            return TypeParserResult<Tag>.Failed($"The tag **{value}** doesn't exist in this guild. " +
                                                    $"Try using the `{CommandHelper.FormatUsage(ctx, context.Services.Get<CommandService>().GetCommand("Tags List"))}` command to see all tags in this guild.");
        }
    }
}