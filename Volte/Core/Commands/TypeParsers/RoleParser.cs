using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.TypeParsers {
    public sealed class RoleParser<TRole> : TypeParser<TRole> where TRole : SocketRole {
        public override Task<TypeParserResult<TRole>> ParseAsync(
            string value, ICommandContext context, IServiceProvider provider) {
            var ctx = (VolteContext)context;
            TRole role = null;
            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseRole(value, out id)) {
                role = ctx.Guild.GetRole(id) as TRole;
            }

            if (role is null) {
                var match = ctx.Guild.Roles.Where(x => x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1) {
                    return Task.FromResult(TypeParserResult<TRole>.Unsuccessful(
                            "Multiple roles found. Try mentioning the role or using its ID.")
                        );
                }

                role = match.FirstOrDefault() as TRole;
            }

            return role is null
                ? Task.FromResult(TypeParserResult<TRole>.Unsuccessful("Role not found."))
                : Task.FromResult(TypeParserResult<TRole>.Successful(role));
        }
    }
}