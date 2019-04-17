using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.TypeParsers
{
    public sealed class BooleanParser : TypeParser<bool>
    {
        private readonly string[] _matchingTrueValues =
        {
            "true", "y", "yes", "ye", "yep", "yeah", "sure", "affirmative", "yar", "aff", "ya", "da", "yas", "enable",
            "yip",
            "positive", "1"
        };

        private readonly string[] _matchingFalseValues =
        {
            "false", "n", "no", "nah", "na", "nej", "nope", "nop", "neg", "negatory", "disable", "nay", "negative", "0"
        };

        public override Task<TypeParserResult<bool>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            if (_matchingTrueValues.ContainsIgnoreCase(value))
                return Task.FromResult(TypeParserResult<bool>.Successful(true));

            if (_matchingFalseValues.ContainsIgnoreCase(value))
                return Task.FromResult(TypeParserResult<bool>.Successful(false));

            return Task.FromResult(bool.TryParse(value, out var result) 
                ? TypeParserResult<bool>.Successful(result) 
                : TypeParserResult<bool>.Unsuccessful("Failed to parse a boolean (true/false) value."));
        }
    }
}