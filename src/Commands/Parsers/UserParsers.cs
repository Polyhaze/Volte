using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class SocketGuildUserParser : VolteTypeParser<SocketGuildUser>
    {
        public override ValueTask<TypeParserResult<SocketGuildUser>> ParseAsync(string value, VolteContext ctx)
        {
            var users = ctx.Guild.Users.ToList();

            SocketGuildUser user = null;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = users.FirstOrDefault(x => x.Id == id);

            user ??= users.FirstOrDefault(x => x.ToString().EqualsIgnoreCase(value));

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || x.Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1)
                    return Failure(
                        "Multiple users found, try mentioning the user or using their ID.");

                user = match.FirstOrDefault();
            }

            return user is null
                ? Failure("User not found.")
                : Success(user);
        }
    }

    [InjectTypeParser]
    public sealed class RestUserParser : VolteTypeParser<RestUser>
    {
        public override async ValueTask<TypeParserResult<RestUser>> ParseAsync(string value, VolteContext ctx)
        {
            RestUser user = null;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = await ctx.Client.Rest.GetUserAsync(id);
            
            return user is null
                ? Failure("User not found.")
                : Success(user);
        }
    }
    
    [InjectTypeParser]
    public sealed class RestGuildUserParser : VolteTypeParser<RestGuildUser>
    {
        public override async ValueTask<TypeParserResult<RestGuildUser>> ParseAsync(string value, VolteContext ctx)
        {
            RestGuildUser user = null;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
                user = await ctx.Client.Rest.GetGuildUserAsync(ctx.Guild.Id, id);

            return user is null
                ? Failure("User not found.")
                : Success(user);
        }
    }
}