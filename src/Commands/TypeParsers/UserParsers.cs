using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class MemberParser : TypeParser<DiscordMember>
    {
        // TODO In DSharpPlus, this is member cache-dependent. This needs to be async, and will not support plain names of non-mentioned non-cached members.
        public override async ValueTask<TypeParserResult<DiscordMember>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            var users = ctx.Guild.Members;

            DiscordMember user = default;

            if (ulong.TryParse(value, out var id) || MentionHelpers.TryParseUser(value, out id))
                user = users.TryGetValue(id, out var foundMember) ? foundMember : await ctx.Guild.GetMemberAsync(id);

            if (user is null) user = users.FirstOrDefault(x => x.Value.ToString().EqualsIgnoreCase(value)).Value;

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Value.Username.EqualsIgnoreCase(value)
                    || x.Value.Nickname.EqualsIgnoreCase(value)).ToArray();
                if (match.Length > 1)
                    return TypeParserResult<DiscordMember>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID.");

                user = match.FirstOrDefault().Value;
            }

            return user is null
                ? TypeParserResult<DiscordMember>.Unsuccessful("User not found.")
                : TypeParserResult<DiscordMember>.Successful(user);
        }
    }

    [VolteTypeParser]
    public sealed class UserParser : TypeParser<DiscordUser>
    {
        public override async ValueTask<TypeParserResult<DiscordUser>> ParseAsync(
            Parameter parameter,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();

            DiscordUser user = null;

            if (ulong.TryParse(value, out var id) || MentionHelpers.TryParseUser(value, out id))
                user = await ctx.Client.ShardClients.First().Value.GetUserAsync(id);

            return user is null
                ? TypeParserResult<DiscordUser>.Unsuccessful("User not found.")
                : TypeParserResult<DiscordUser>.Successful(user);
        }
    }
}