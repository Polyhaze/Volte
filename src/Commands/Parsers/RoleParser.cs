using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class RoleParser : VolteTypeParser<SocketRole>
    {
        public override ValueTask<TypeParserResult<SocketRole>> ParseAsync(Parameter _, string value,
            VolteContext ctx)
        {
            SocketRole role = default;
            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseRole(value, out id))
                role = ctx.Guild.GetRole(id).Cast<SocketRole>();

            if (role is null)
            {
                var match = ctx.Guild.Roles.Where(x => x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Failure(
                        "Multiple roles found. Try mentioning the role or using its ID.");

                role = match.FirstOrDefault().Cast<SocketRole>();
            }

            return role is null
                ? Failure($"Role {Format.Code(value)} not found.")
                : Success(role);
        }
    }
}