using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Qmmands;

namespace Volte.Core.Commands.TypeParsers
{
    class EmoteParser : TypeParser<IEmote>
    {
        public override Task<TypeParserResult<IEmote>> ParseAsync(string value, ICommandContext context, IServiceProvider provider)
        {
            return Emote.TryParse(value, out var emote)
                ? Task.FromResult(new TypeParserResult<IEmote>(emote))
                : Task.FromResult(Regex.Match(value, @"[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                    ? new TypeParserResult<IEmote>(new Emoji(value))
                    : new TypeParserResult<IEmote>("Emote not found."));
        }
    }
}
