using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class ChannelParser : TypeParser<DiscordChannel>
    {
        public override ValueTask<TypeParserResult<DiscordChannel>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
        {
            var ctx = context.AsVolteContext();
            DiscordChannel channel = null;

            if (ulong.TryParse(value, out var id) || MentionHelpers.TryParseChannel(value, out id))
                channel = ctx.Client.FindFirstChannel(id);
    
            if (channel is null)
            {
                var match = ctx.Guild.GetTextChannels().Where(x => x.Name.EqualsIgnoreCase(value))
                    .ToList();
                if (match.Count > 1)
                    return TypeParserResult<DiscordChannel>.Unsuccessful(
                        "Multiple channels found. Try mentioning the channel or using its ID.");
            }

            return channel is null
                ? TypeParserResult<DiscordChannel>.Unsuccessful("Channel not found.")
                : TypeParserResult<DiscordChannel>.Successful(channel);
        }
    }
}