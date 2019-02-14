using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.TypeParsers {
    public class ChannelParser<TChannel> : TypeParser<TChannel> where TChannel : SocketTextChannel {
        public override Task<TypeParserResult<TChannel>> ParseAsync(
            string value, ICommandContext context, IServiceProvider provider) {
            var ctx = (VolteContext)context;
            TChannel channel = null;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseChannel(value, out id)) {
                channel = ctx.Guild.TextChannels.FirstOrDefault(x => x.Id.Equals(id)) as TChannel;
            }

            if (channel is null) {
                var match = ctx.Guild.TextChannels.Where(x => x.Name.EqualsIgnoreCase(value)).ToList();
                if (match.Count > 1) {
                    return Task.FromResult(TypeParserResult<TChannel>.Unsuccessful(
                            "Multiple channels found. Try mentioning the channel or using its ID.")
                        );
                }
            }

            return channel is null
                ? Task.FromResult(TypeParserResult<TChannel>.Unsuccessful("Channel not found."))
                : Task.FromResult(TypeParserResult<TChannel>.Successful(channel));
        }
    }
}