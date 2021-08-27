using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands
{
    [InjectTypeParser]
    public class TimeSpanParser : VolteTypeParser<TimeSpan>
    {
        private static readonly Regex TimeSpanRegex = new Regex(
            @"(?<Years>\d{1}y\s*)?(?<Weeks>\d+w\s*)?(?<Days>\d+d\s*)?(?<Hours>\d+h\s*)?(?<Minutes>\d+m\s*)?(?<Seconds>\d+s\s*)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);

        public override ValueTask<TypeParserResult<TimeSpan>> ParseAsync(string value, VolteContext _)
        {
            if (!TimeSpanRegex.IsMatch(value, out var match))
                return Failure("Content contained no valid TimeSpan expressions.");

            var r = ..^1;
            var result = new TimeSpan();

            if (match.Groups["Years"].Success && int.TryParse(match.Groups["Years"].Value[r], out var years))
                result = result.Add((years * 365).Days());

            if (match.Groups["Weeks"].Success && int.TryParse(match.Groups["Weeks"].Value[r], out var weeks))
                result = result.Add((weeks * 7).Days());
            
            if (match.Groups["Days"].Success && int.TryParse(match.Groups["Days"].Value[r], out var days))
                result = result.Add(days.Days());
            
            if (match.Groups["Hours"].Success && int.TryParse(match.Groups["Hours"].Value[r], out var hours))
                result = result.Add(hours.Hours());

            if (match.Groups["Minutes"].Success && int.TryParse(match.Groups["Minutes"].Value[r], out var minutes))
                result = result.Add(minutes.Minutes());

            if (match.Groups["Seconds"].Success && int.TryParse(match.Groups["Seconds"].Value[r], out var seconds))
                result = result.Add(seconds.Seconds());

            return Success(result);
        }
    }
}