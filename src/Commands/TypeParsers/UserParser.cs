using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    public sealed class UserParser<TUser> : TypeParser<TUser> where TUser : IUser
    {
        public override Task<TypeParserResult<TUser>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var users = ctx.Guild.Users.OfType<TUser>().ToList();

            TUser user = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = users.FirstOrDefault(x => x.Id == id);

            if (user is null) user = users.FirstOrDefault(x => x.ToString().EqualsIgnoreCase(value));

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || x.Cast<IGuildUser>().Nickname.EqualsIgnoreCase(value)).ToList();
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