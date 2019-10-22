using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class SocketGuildUserParser : TypeParser<SocketGuildUser>
    {
        public override ValueTask<TypeParserResult<SocketGuildUser>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context,
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
                    || x.Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return TypeParserResult<SocketGuildUser>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID.");

                user = match.FirstOrDefault();
            }

            return user is null
                ? TypeParserResult<SocketGuildUser>.Unsuccessful("User not found.")
                : TypeParserResult<SocketGuildUser>.Successful(user);
        }
    }

    [VolteTypeParser]
    public sealed class RestUserParser : TypeParser<RestUser>
    {
        public override async ValueTask<TypeParserResult<RestUser>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();

            RestUser user = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = await ctx.Client.Shards.First().Rest.GetUserAsync(id);

            

            return user is null
                ? TypeParserResult<RestUser>.Unsuccessful("User not found.")
                : TypeParserResult<RestUser>.Successful(user);
        }
    }
}