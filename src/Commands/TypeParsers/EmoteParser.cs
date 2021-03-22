using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [VolteTypeParser]
    public sealed class EmoteParser : TypeParser<IEmote>
    {
        public override ValueTask<TypeParserResult<IEmote>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context) 
            => Emote.TryParse(value, out var emote)
                ? TypeParserResult<IEmote>.Successful(emote)
                : Regex.Match(value, "[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                    ? TypeParserResult<IEmote>.Successful(new Emoji(value))
                    : TypeParserResult<IEmote>.Failed("Emote not found.");
    }
}