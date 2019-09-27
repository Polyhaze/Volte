using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class ChannelParser : TypeParser<SocketTextChannel>
    {
        public override ValueTask<TypeParserResult<SocketTextChannel>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            SocketTextChannel channel = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseChannel(value, out id))
                channel = ctx.Client.GetChannel(id).Cast<SocketTextChannel>();

            if (channel is null)
            {
                var match = ctx.Guild.TextChannels.Where(x => x.Name.EqualsIgnoreCase(value))
                    .ToList();
                if (match.Count > 1)
                    return TypeParserResult<SocketTextChannel>.Unsuccessful(
                        "Multiple channels found. Try mentioning the channel or using its ID.");
            }

            return channel is null
                ? TypeParserResult<SocketTextChannel>.Unsuccessful("Channel not found.")
                : TypeParserResult<SocketTextChannel>.Successful(channel);
        }
    }
}