using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.TypeParsers
{
    public sealed class RoleParser<TRole> : TypeParser<TRole> where TRole : DiscordRole
    {
        public override Task<TypeParserResult<TRole>> ParseAsync(
            Parameter param,
            string value, 
            ICommandContext context, 
            IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            TRole role = null;
            if (ulong.TryParse(value, out var id))
                role = ctx.Guild.GetRole(id) as TRole;

            if (role is null) role = ctx.Message.MentionedRoles.FirstOrDefault() as TRole;

            if (role is null)
            {
                var match = ctx.Guild.Roles.Where(x => x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<TRole>.Unsuccessful(
                        "Multiple roles found. Try mentioning the role or using its ID.")
                    );

                role = match.FirstOrDefault() as TRole;
            }

            return role is null
                ? Task.FromResult(TypeParserResult<TRole>.Unsuccessful("Role not found."))
                : Task.FromResult(TypeParserResult<TRole>.Successful(role));
        }
    }
}