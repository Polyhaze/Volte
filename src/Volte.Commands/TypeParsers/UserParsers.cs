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
            Parameter _,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            var users = ctx.Guild.Members;

            DiscordMember member = default;

            if (ulong.TryParse(value, out var id) || MentionHelper.TryParseUser(value, out id))
                member = users.TryGetValue(id, out var foundMember) ? foundMember : await ctx.Guild.GetMemberAsync(id);

            if (member is null) member = users.FirstOrDefault(x => x.Value.ToString().EqualsIgnoreCase(value)).Value;

            if (member is null)
            {
                var match = users.Values.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || x.Nickname.EqualsIgnoreCase(value)).ToArray();
                if (match.Length > 1)
                    return TypeParserResult<DiscordMember>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID.");

                member = match.FirstOrDefault();
            }

            return member is null
                ? TypeParserResult<DiscordMember>.Unsuccessful("User not found.")
                : TypeParserResult<DiscordMember>.Successful(member);
        }
    }

    [VolteTypeParser]
    public sealed class UserParser : TypeParser<DiscordUser>
    {
        public override async ValueTask<TypeParserResult<DiscordUser>> ParseAsync(
            Parameter _,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();

            DiscordUser user = null;

            if (ulong.TryParse(value, out var id) || MentionHelper.TryParseUser(value, out id))
                user = await ctx.Client.ShardClients[ctx.Client.GetShardId(ctx.Guild.Id)].GetUserAsync(id);

            return user is null
                ? TypeParserResult<DiscordUser>.Unsuccessful("User not found.")
                : TypeParserResult<DiscordUser>.Successful(user);
        }
    }
}