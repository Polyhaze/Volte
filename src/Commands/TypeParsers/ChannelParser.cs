using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class ChannelParser : TypeParser<SocketTextChannel>
    {
        public override ValueTask<TypeParserResult<SocketTextChannel>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
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
                    return TypeParserResult<SocketTextChannel>.Failed(
                        "Multiple channels found. Try mentioning the channel or using its ID.");
                channel = match.First();
            }

            return channel is null
                ? TypeParserResult<SocketTextChannel>.Failed("Channel not found.")
                : TypeParserResult<SocketTextChannel>.Successful(channel);
        }
    }
}