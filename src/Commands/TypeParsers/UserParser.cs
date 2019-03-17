using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.TypeParsers
{
    public sealed class UserParser<TUser> : TypeParser<TUser> where TUser : DiscordUser
    {
        public override Task<TypeParserResult<TUser>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            var users = ctx.Guild.Members.OfType<TUser>().ToList();

            TUser user = null;

            if (ulong.TryParse(value, out var id))
                user = users.FirstOrDefault(x => x.Id == id);

            if (user is null) user = ctx.Message.MentionedUsers.FirstOrDefault() as TUser;

            if (user is null) user = users.FirstOrDefault(x => x.ToString().EqualsIgnoreCase(value));

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || (x as DiscordMember).Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<TUser>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID."));

                user = match.FirstOrDefault();
            }

            return Task.FromResult(user is null
                ? TypeParserResult<TUser>.Unsuccessful("User not found.")
                : TypeParserResult<TUser>.Successful(user));
        }
    }
}