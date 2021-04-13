using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [VolteTypeParser]
    public sealed class UnixParser : TypeParser<Dictionary<string, string>>
    {
        private enum ParserState
        {
            Neutral,
            ArgumentName,
            ArgumentValue
        }

        public override ValueTask<TypeParserResult<Dictionary<string, string>>> ParseAsync(Parameter _, string value, CommandContext __)
        {
            var state = ParserState.Neutral;
            var inQuote = false;
            var result = new Dictionary<string, string>();
            var argName = new StringBuilder();
            var argVal = new StringBuilder();
            foreach (var token in value)
            {
                switch (token)
                {
                    case '-':
                        state = ParserState.ArgumentName;
                        break;
                    case '=' when state is ParserState.ArgumentName:
                        state = ParserState.ArgumentValue;
                        break;
                    case ' ' when inQuote:
                        argVal.Append(' ');
                        break;
                    case ' ' when state is ParserState.ArgumentName:
                        state = ParserState.ArgumentValue;
                        break;
                    case ' ' when state is ParserState.ArgumentValue:
                        state = ParserState.Neutral;
                        result.Add(argName.ToString(), argVal.ToString());
                        argName.Clear();
                        argVal.Clear();
                        break;
                    case '"' when state is ParserState.ArgumentValue && !inQuote:
                        inQuote = true;
                        break;
                    case '"' when state == ParserState.ArgumentValue && inQuote:
                        inQuote = false;
                        result.Add(argName.ToString(), argVal.ToString());
                        argName.Clear();
                        argVal.Clear();
                        state = ParserState.Neutral;
                        break;
                    case '"':
                        return TypeParserResult<Dictionary<string, string>>.Failed("Unexpected quote.");
                    default:
                        switch (state)
                        {
                            case ParserState.ArgumentName:
                                argName.Append(token.ToString().ToLower());
                                break;
                            case ParserState.ArgumentValue:
                                argVal.Append(token);
                                break;
                        }

                        break;
                }
            }

            if (argName.Length > 0 && argVal.Length > 0)
                result.Add(argName.ToString(), argVal.ToString());

            return result.IsEmpty()
                ? TypeParserResult<Dictionary<string, string>>.Failed("No Unix-style arguments were parsed.")
                : TypeParserResult<Dictionary<string, string>>.Successful(result);
        }
    }
}