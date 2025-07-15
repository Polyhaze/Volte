namespace Volte.Helpers;

#nullable enable

public struct VolteExpression
{
    public string? PropertyName { get; private set; }
    public PredicateType PredicateType { get; private set; }
    public string ComparatorValue { get; private set; }

    public bool Matches<T>(T value)
    {
        string? val = 
            PropertyName != null 
                ? Mirror.Reflect(value)
                    .Get<object>(PropertyName, BindingFlags.Public | BindingFlags.Instance)?
                    .ToString()
                : value?.ToString();

        if (val is null)
            throw new NullReferenceException("Cannot use a null reference as a base for comparison.");

        return GetComparator(val)(ComparatorValue);
    }


    public static VolteExpression Compile(string expr)
    {
        var parts = expr.Split(' ');
        
        switch (parts.Length)
        {
            case < 2:
                throw new FormatException("Input string has less than 2 parts, indicating predicate type and value. For example: 'equals hi'");
            case > 3:
                throw new FormatException("Input string has more than 3 parts, indicating object property, predicate type and value. For example: 'Name contains Jane'");
        }

        VolteExpression result = new();

        switch (parts.Length)
        {
            case 2:
                result.PredicateType = EnumHelpers.ParsePredicateType(parts[0]);
                result.ComparatorValue = parts[1];
                break;
            case 3:
                result.PropertyName = parts[0];
                result.PredicateType = EnumHelpers.ParsePredicateType(parts[1]);
                result.ComparatorValue = parts[2];
                break;
            default:
                throw new FormatException(); // shouldn't happen (above checks), so there's no message.
        }

        return result;
    }
    
    private Func<string, bool> GetComparator(string source) =>
        PredicateType switch
        {
            PredicateType.Equals => source.Equals,
            PredicateType.EqualsIgnoreCase => source.EqualsIgnoreCase,
            PredicateType.Contains => source.Contains,
            PredicateType.ContainsIgnoreCase => source.ContainsIgnoreCase,
            PredicateType.StartsWith => source.StartsWith,
            PredicateType.StartsWithIgnoreCase => source.StartsWithIgnoreCase,
            PredicateType.EndsWith => source.EndsWith,
            PredicateType.EndsWithIgnoreCase => source.EndsWithIgnoreCase,
            _ => throw new ArgumentOutOfRangeException(nameof(PredicateType))
        };
}

public static partial class EnumHelpers
{
    public static PredicateType ParsePredicateType(string value)
        => value.ToLower() switch
        {
            "eq" or "equals" => PredicateType.Equals,
            "eqi" or "eqic" or "equalsi" or "equalsic" or "equalsignorecase" => PredicateType.EqualsIgnoreCase,
            "c" or "con" or "contains" => PredicateType.Contains,
            "ci" or "cic" or "coni" or "conic" or "containsi" or "containsic" or "containsignorecase" => PredicateType.ContainsIgnoreCase,
            "s" or "sw" or "startswith" => PredicateType.StartsWith,
            "si" or "sic" or "swi" or "swic" or "startswithi" or "startswithic" or "startswithignorecase" => PredicateType.StartsWithIgnoreCase,
            "e" or "ew" or "endswith" => PredicateType.EndsWith,
            "ei" or "eic" or "ewi" or "ewic" or "endswithi" or "endswithic" or "endswithignorecase" => PredicateType.EndsWithIgnoreCase,
            _ => throw new FormatException($"Unrecognized {nameof(PredicateType)} input: '{value}'")
        };
}

public enum PredicateType
{
    Equals,
    EqualsIgnoreCase,
    Contains,
    ContainsIgnoreCase,
    StartsWith,
    StartsWithIgnoreCase,
    EndsWith,
    EndsWithIgnoreCase
}