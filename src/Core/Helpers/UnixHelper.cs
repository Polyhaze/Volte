using System;
using System.Collections.Generic;
using System.Linq;
using Gommon;

namespace Volte.Core.Helpers
{
    public static class UnixHelper
    {
        private enum ParsingState
        {
            Neutral,
            ArgumentName,
            ArgumentValue
        }

        /// <summary>
        ///     Attempts to parse the given string into a readable <see cref="Dictionary{TKey,TValue}"/>; and will return false if parsing fails.
        /// </summary>
        /// <param name="input">The unix command string to parse.</param>
        /// <param name="output">The dictionary of all parsed arguments and their values.</param>
        /// <returns>True if parsing succeeded; false otherwise.</returns>
        public static bool TryParseNamedArguments(string input, out (Dictionary<string, string> Parsed, Exception Error) output)
        {
            try
            {
                output = (ParseNamedArguments(input), null);
                return true;
            }
            catch (Exception e)
            {
                output = (null, e);
                return false;
            }
        }

        /// <summary>
        ///     Parses a "-unix command" string into a readable <see cref="Dictionary{TKey,TValue}"/>.
        ///     This does not have any specific requirements; if it's a basic "-unix 'command string'" it's parseable.
        /// </summary>
        /// <param name="input">The unix command string to parse.</param>
        /// <returns>A Dictionary keyed by the argument name, and valued according to its corresponding value.</returns>
        /// <exception cref="InvalidOperationException">When the resulting <see cref="Dictionary{TKey,TValue}"/> is empty after parsing.</exception>
        /// <exception cref="ArgumentException">Content contains an unexpected quotation mark.</exception>
        public static Dictionary<string, string> ParseNamedArguments(string input)
        {
            var state = ParsingState.Neutral;
            var inQuote = false;
            var result = new Dictionary<string, string>();
            var argName = string.Empty;
            var argVal = string.Empty;
            var lastChar = new char();
            input.ForEachIndexed((token, index) =>
            {
                switch (token)
                {
                    case '-' when state is ParsingState.ArgumentName && lastChar is ' ':
                        state = ParsingState.Neutral;
                        result.Add(argName, true.ToString());
                        argName = string.Empty;
                        break;
                    case '-' when state is ParsingState.ArgumentValue:
                        state = ParsingState.Neutral;
                        result.Add(argName, true.ToString());
                        argName = string.Empty;
                        break;
                    case '-':
                        state = ParsingState.ArgumentName;
                        break;
                    case '=' when state is ParsingState.ArgumentName:
                        state = ParsingState.ArgumentValue;
                        break;
                    case ' ' when inQuote:
                        argVal += ' ';
                        break;
                    case ' ' when state is ParsingState.ArgumentName && (input.ElementAtOrDefault(index + 1) is '-' || input[index..].All(x => x != ' ')):
                        state = ParsingState.Neutral;
                        result.Add(argName, true.ToString());
                        argName = string.Empty;
                        break;
                    case ' ' when state is ParsingState.ArgumentName:
                        state = ParsingState.ArgumentValue;
                        break;
                    case ' ' when state is ParsingState.ArgumentValue:
                        state = ParsingState.Neutral;
                        result.Add(argName, argVal);
                        argName = string.Empty;
                        argVal = string.Empty;
                        break;
                    case '"' when state is ParsingState.ArgumentValue && !inQuote:
                        inQuote = true;
                        break;
                    case '"' when state is ParsingState.ArgumentValue && inQuote:
                        inQuote = false;
                        result.Add(argName, argVal);
                        argName = string.Empty;
                        argVal = string.Empty;
                        state = ParsingState.Neutral;
                        break;
                    case '"': throw new ArgumentException("Content contained an unexpected quotation mark.");
                    default:
                        // ReSharper disable once ConvertIfStatementToSwitchStatement
                        if (state is ParsingState.ArgumentName)
                            argName += char.ToLower(token);
                        else if (state is ParsingState.ArgumentValue)
                            argVal += token;
                        break;
                }
                lastChar = token;
            });

            if (!argName.IsEmpty())
                result.Add(argName, argVal);

            return !result.IsEmpty() 
                ? result 
                : throw new InvalidOperationException("No Unix-style arguments were recognizable from the input value.");
        }
    }
}
