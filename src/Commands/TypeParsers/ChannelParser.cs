using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    public sealed class ChannelParser<TChannel> : TypeParser<TChannel> where TChannel : ITextChannel
    {
        public override Task<TypeParserResult<TChannel>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            TChannel channel = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseChannel(value, out id))
                channel = ctx.Guild.GetTextChannel(id).Cast<TChannel>();

            if (channel is null)
            {
                var match = ctx.Guild.TextChannels.Where(x => x.Name.EqualsIgnoreCase(value))
                    .ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<TChannel>.Unsuccessful(
                        "Multiple channels found. Try mentioning the channel or using its ID."));
            }

            return Task.FromResult(channel is null
                ? TypeParserResult<TChannel>.Unsuccessful("Channel not found.")
                : TypeParserResult<TChannel>.Successful(channel));
        }
    }
}