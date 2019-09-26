using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser(true)]
    public sealed class BooleanParser : TypeParser<bool>
    {
        private IEnumerable<string> TrueValues =>
            new List<string>
            {
                "true", "y", "yes", "ye", "yep", "yeah", "sure", "affirmative", "yar", "aff", "ya", "da", "yas",
                "enable", "yip", "positive", "1"
            };

        private IEnumerable<string> FalseValues =>
            new List<string>
            {
                "false", "n", "no", "nah", "na", "nej", "nope", "nop", "neg", "negatory", "disable", "nay", "negative",
                "0"
            };

        public override ValueTask<TypeParserResult<bool>> ParseAsync(
            Parameter param,
            string value,
            CommandContext context,
            IServiceProvider provider)
        {
            if (TrueValues.ContainsIgnoreCase(value))
                return TypeParserResult<bool>.Successful(true);

            if (FalseValues.ContainsIgnoreCase(value))
                return TypeParserResult<bool>.Successful(false);

            return bool.TryParse(value, out var result)
                ? TypeParserResult<bool>.Successful(result)
                : TypeParserResult<bool>.Unsuccessful($"Failed to parse a {typeof(bool)} (true/false) value.");
        }
    }
}