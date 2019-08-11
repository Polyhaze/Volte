using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class EmoteParser : TypeParser<IEmote>
    {
        public override ValueTask<TypeParserResult<IEmote>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context,
            IServiceProvider provider) 
            => new ValueTask<TypeParserResult<IEmote>>(Emote.TryParse(value, out var emote)
                ? new TypeParserResult<IEmote>(emote)
                : Regex.Match(value, "[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                    ? new TypeParserResult<IEmote>(new Emoji(value))
                    : new TypeParserResult<IEmote>("Emote not found."));
    }
}