using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public sealed class EmoteParser : VolteTypeParser<IEmote>
    {
        public override ValueTask<TypeParserResult<IEmote>> ParseAsync(string value, VolteContext _)
            => Emote.TryParse(value, out var emote)
                ? Success(emote)
                : Regex.Match(value, "[^\u0000-\u007F]+", RegexOptions.IgnoreCase).Success
                    ? Success(new Emoji(value))
                    : Failure("Emote not found.");
    }
}