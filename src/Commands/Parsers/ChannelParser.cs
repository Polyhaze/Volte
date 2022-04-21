using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class ChannelParser : VolteTypeParser<SocketTextChannel>
    {
        public override ValueTask<TypeParserResult<SocketTextChannel>> ParseAsync(string value, VolteContext ctx)
        {
            SocketTextChannel channel = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseChannel(value, out id))
                channel = ctx.Client.GetChannel(id).Cast<SocketTextChannel>();

            if (channel is null)
            {
                var match = ctx.Guild.TextChannels.Where(x => x.Name.EqualsIgnoreCase(value))
                    .ToList();
                if (match.Count > 1)
                    return Failure(
                        "Multiple channels found. Try mentioning the channel or using its ID.");
                channel = match.First();
            }

            return channel is null
                ? Failure("Channel not found.")
                : Success(channel);
        }
    }
}