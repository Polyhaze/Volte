using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.TypeParsers
{
    public sealed class UserParser<TUser> : TypeParser<TUser> where TUser : SocketUser
    {
        public override Task<TypeParserResult<TUser>> ParseAsync(
            string value, ICommandContext context, IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            var type = typeof(TUser);
            List<TUser> users;
            users = ctx.Guild.Users.OfType<TUser>().ToList();

            TUser user = null;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = users.FirstOrDefault(x => x.Id == id);

            if (user is null) user = users.FirstOrDefault(x => x.ToString().EqualsIgnoreCase(value));

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || (x as SocketGuildUser).Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<TUser>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID.")
                    );

                user = match.FirstOrDefault();
            }

            return user is null
                ? Task.FromResult(TypeParserResult<TUser>.Unsuccessful("User not found."))
                : Task.FromResult(TypeParserResult<TUser>.Successful(user));
        }
    }
}