using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class RoleParser : TypeParser<DiscordRole>
    {
        public override ValueTask<TypeParserResult<DiscordRole>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            DiscordRole role = null;

            if (ulong.TryParse(value, out var id) || MentionHelper.TryParseRole(value, out id))
                role = ctx.Guild.GetRole(id);

            if (role is null)
            {
                var match = ctx.Guild.Roles.Where(x => x.Value.Name.EqualsIgnoreCase(value)).ToArray();
                if (match.Length > 1)
                    return TypeParserResult<DiscordRole>.Unsuccessful(
                        "Multiple roles found with that name. Try mentioning the specific role or using its ID.");

                role = match.FirstOrDefault().Value;
            }

            return role is null
                ? TypeParserResult<DiscordRole>.Unsuccessful($"Role `{value}` not found.")
                : TypeParserResult<DiscordRole>.Successful(role);
        }
    }
}