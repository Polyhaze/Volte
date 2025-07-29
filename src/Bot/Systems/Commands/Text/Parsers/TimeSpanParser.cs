using System.Text.RegularExpressions;

namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public partial class TimeSpanParser : VolteTypeParser<TimeSpan>
{
    public override ValueTask<TypeParserResult<TimeSpan>> ParseAsync(string value, VolteContext _) => Parse(value);
        
    public static TypeParserResult<TimeSpan> Parse(string value)
    {
        if (!TimeSpanRegex.IsMatch(value, out var match))
            return Failure("Content contained no valid TimeSpan expressions.");

        var r = ..^1;
        var result = new TimeSpan();

        if (match.Groups["Years"].Success && match.Groups["Years"].Value[r].TryParse<int>(out var years))
            result += (years * 365).Days();

        if (match.Groups["Weeks"].Success && match.Groups["Weeks"].Value[r].TryParse<int>(out var weeks))
            result += (weeks * 7).Days();
            
        if (match.Groups["Days"].Success && match.Groups["Days"].Value[r].TryParse<int>(out var days))
            result += days.Days();
            
        if (match.Groups["Hours"].Success && match.Groups["Hours"].Value[r].TryParse<int>(out var hours))
            result += hours.Hours();

        if (match.Groups["Minutes"].Success && match.Groups["Minutes"].Value[r].TryParse<int>(out var minutes))
            result += minutes.Minutes();

        if (match.Groups["Seconds"].Success && match.Groups["Seconds"].Value[r].TryParse<int>(out var seconds))
            result += seconds.Seconds();

        return Success(result);
    }
        
    private static readonly Regex TimeSpanRegex = GeneratedTimeSpanRegex();

    [GeneratedRegex(@"(?<Years>\d{1}y\s*)?(?<Weeks>\d+w\s*)?(?<Days>\d+d\s*)?(?<Hours>\d+h\s*)?(?<Minutes>\d+m\s*)?(?<Seconds>\d+s\s*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript, "en-US")]
    private static partial Regex GeneratedTimeSpanRegex();
}