using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BrackeysBot
{
    public class AbbreviatedTimeSpanTypeReader : TypeReader
    {
        // Regex to match the timespan (years,months,weeks,days,hours,minutes,seconds)
        private static readonly Regex _pattern =
            new Regex(@"^(?:(\d+)y)?(?:(\d+)mo)?(?:(\d+)w)?(?:(\d+)d)?(?:(\d+)h)?(?:(\d+)m)?(?:(\d+)s?)?$");

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var match = _pattern.Match(input);
            if (match.Success)
            {
                var groups = match.Groups;

                int parseAt(int i)
                {
                    int.TryParse(groups[i].Value, out int v);
                    return v;
                }

                TimeSpan result = new TimeSpan(
                    parseAt(1) * 365 + parseAt(2) * 30 + parseAt(3) * 7 + parseAt(4),
                    parseAt(5),
                    parseAt(6),
                    parseAt(7));

                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as abbreviated timespan."));
        }
    }
}
