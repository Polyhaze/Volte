using System.Collections.Generic;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [VolteTypeParser(true)]
    public sealed class BooleanParser : TypeParser<bool>
    {
        private string[] _trueValues =
        {
            "true", "y",
            "yes", "ye",
            "yep", "yeah",
            "sure", "affirmative",
            "yar", "aff",
            "ya", "da",
            "yas", "enable",
            "yip", "positive",
            "1"
        };

        private readonly string[] _falseValues =
        {
            "false", "n",
            "no", "nah",
            "na", "nej",
            "nope", "nop",
            "neg", "negatory",
            "disable", "nay",
            "negative", "0"
        };

        public override ValueTask<TypeParserResult<bool>> ParseAsync(Parameter _, string value, CommandContext __)
        {
            if (_trueValues.ContainsIgnoreCase(value))
                return TypeParserResult<bool>.Successful(true);

            if (_falseValues.ContainsIgnoreCase(value))
                return TypeParserResult<bool>.Successful(false);

            return bool.TryParse(value, out var result)
                ? TypeParserResult<bool>.Successful(result)
                : TypeParserResult<bool>.Failed(
                    $"Failed to parse a {typeof(bool)} (true/false) value. Try using true or false.");
        }
    }
}