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
                .GetProperty("UnicodeEmojis", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        
        private static readonly Regex EmojiRegex = new Regex(@"^<a?:\w+:(?<id>\d+)>$", RegexOptions.Compiled);
        
        public override ValueTask<TypeParserResult<DiscordEmoji>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context)
        {
            // Use our DisgustingDictionary to parse an unicode emoji
            if (UnicodeEmojis.TryGetValue(value, out var unicodeEmoji))
            {
                return TypeParserResult<DiscordEmoji>.Successful(DiscordEmoji.FromUnicode(unicodeEmoji));
            }
            
            // Attempt to parse guild emotes (by expression)
            var shards = context.AsVolteContext().Client.ShardClients;

            if (value[0] == '<' && value[^1] == '>')
            {
                var match = EmojiRegex.Match(value);
                
                if (match.Success)
                {
                    var id = ulong.Parse(match.Groups["id"].Value);

                    foreach (var emojis in shards
                        .SelectMany(e => e.Value.Guilds)
                        .Select(e => e.Value.Emojis))
                    {
                        if (emojis.TryGetValue(id, out var emoji))
                            return TypeParserResult<DiscordEmoji>.Successful(emoji);
                    }
                }
            }

            // Attempt to parse guild emotes (by name)
            if (value[0] == ':' && value[^1] == ':')
            {
                var nameSanitized = value.Substring(1, value.Length - 2);
                foreach (var (_, emoji) in shards
                    .SelectMany(e => e.Value.Guilds)
                    .SelectMany(e => e.Value.Emojis))
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