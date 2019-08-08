using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gommon;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

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

        public override Task<TypeParserResult<bool>> ParseAsync(
            Parameter param,
            string value,
            ICommandContext context,
            IServiceProvider provider)
        {
            if (TrueValues.ContainsIgnoreCase(value))
                return Task.FromResult(TypeParserResult<bool>.Successful(true));

            if (FalseValues.ContainsIgnoreCase(value))
                return Task.FromResult(TypeParserResult<bool>.Successful(false));

            return Task.FromResult(bool.TryParse(value, out var result)
                ? TypeParserResult<bool>.Successful(result)
                : TypeParserResult<bool>.Unsuccessful($"Failed to parse a {typeof(bool)} (true/false) value."));
        }
    }
}