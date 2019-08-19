using System;
using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Models.Guild;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class TagParser : TypeParser<Tag>
    {
        public override ValueTask<TypeParserResult<Tag>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var tag = ctx.GuildData.Extras.Tags.FirstOrDefault(x => x.Name.EqualsIgnoreCase(value));

            return tag is null 
                ? TypeParserResult<Tag>.Unsuccessful($"The tag **{value}** doesn't exist in this guild. " +
                                                     $"Try using the `{ctx.GuildData.Configuration.CommandPrefix}tags` command to see all tags in this guild.") 
                : TypeParserResult<Tag>.Successful(tag);

        }
    }
}