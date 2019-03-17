using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.TypeParsers
{
    public sealed class ChannelParser<TChannel> : TypeParser<TChannel> where TChannel : DiscordChannel
    {
        public override Task<TypeParserResult<TChannel>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            TChannel channel = null;

            if (ulong.TryParse(value, out var id))
                channel = ctx.Guild.Channels.FirstOrDefault(x => x.Id == id) as TChannel;

            if (channel is null)
            {
                channel = ctx.Message.MentionedChannels.FirstOrDefault() as TChannel;
            }

            if (channel is null)
            {
                var match = ctx.Guild.Channels.Where(x => x.Name.EqualsIgnoreCase(value))
                    .ToList();
                if (match.Count > 1)
                    return Task.FromResult(TypeParserResult<TChannel>.Unsuccessful(
                        "Multiple channels found. Try mentioning the channel or using its ID."));
            }


            return Task.FromResult(channel is null
                ? TypeParserResult<TChannel>.Unsuccessful("Channel not found.")
                : TypeParserResult<TChannel>.Successful(channel));
            ;
        }
    }
}