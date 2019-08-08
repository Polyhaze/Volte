using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class UserParser : TypeParser<SocketGuildUser>
    {
        public override Task<TypeParserResult<SocketGuildUser>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var users = ctx.Guild.Users.ToList();

            SocketGuildUser user = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = users.FirstOrDefault(x => x.Id == id);

            if (user is null) user = users.FirstOrDefault(x => x.ToString().EqualsIgnoreCase(value));

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || x.Cast<IGuildUser>().Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<SocketGuildUser>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID."));

                user = match.FirstOrDefault();
            }

            return Task.FromResult(user is null
                ? TypeParserResult<SocketGuildUser>.Unsuccessful("User not found.")
                : TypeParserResult<SocketGuildUser>.Successful(user));
        }
    }
}