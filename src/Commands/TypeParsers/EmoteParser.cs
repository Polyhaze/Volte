using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class EmoteParser : TypeParser<DiscordEmoji>
    {
        // DISGUSTING. Can we PR a built-in impl for this for sharded clients?
        private static readonly IReadOnlyDictionary<string, string> UnicodeEmojis
            = (IReadOnlyDictionary<string, string>) typeof(DiscordEmoji)
                .GetProperty("UnicodeEmojis", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(null);
        
        public override ValueTask<TypeParserResult<DiscordEmoji>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
        {
            // Use our DisgustingDictionary to parse an unicode emoji
            if (UnicodeEmojis.ContainsKey(value))
            {
                return TypeParserResult<DiscordEmoji>.Successful(DiscordEmoji.FromUnicode(value));
            }
            
            // Attempt to parse guild emotes
            foreach (var (_, shard) in context.AsVolteContext().Client.ShardClients)
            {
                var nameSanitized = value.Substring(1, value.Length - 2);
                foreach (var emoji in shard.Guilds.Select(e => e.Value).SelectMany(xg => xg.Emojis.Select(e => e.Value)))
                {
                    if (emoji.Name == nameSanitized)
                        return TypeParserResult<DiscordEmoji>.Successful(emoji);
                }
            }
            
            // If all that fails, try to detect if it *might* be an emote and fail otherwise.
            return Regex.Match(value, "[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                ? TypeParserResult<DiscordEmoji>.Successful(DiscordEmoji.FromUnicode(value))
                : TypeParserResult<DiscordEmoji>.Unsuccessful("Emote not found.");
        }
    }
}