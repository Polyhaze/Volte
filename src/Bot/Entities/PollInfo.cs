

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Entities;

public sealed class PollInfo
{
    public static PollInfo FromFields(params (object Name, object Value)[] fields) => new PollInfo().AddFields(fields);

    public static PollInfo FromFields(IEnumerable<(object Name, object Value)> fields) =>
        new PollInfo().AddFields(fields);

    public static bool TryParse(string raw, out PollInfo result)
    {
        result = Parse(raw);
        return result.Validation.IsValid;
    }
        
    public static PollInfo Parse(string raw) => Parse(raw.Split(';', StringSplitOptions.RemoveEmptyEntries));

    public static PollInfo Parse(string[] choices)
        => FromDefaultFields(choices).WithPrompt(choices.First());

    public static PollInfo FromDefaultFields(IEnumerable<string> choices)
    {
        var emojis = DiscordHelper.GetPollEmojis();
        var collection = choices as string[] ?? choices.ToArray();
        if (collection.Length - 1 > 10)
            return FromInvalid("More than 9 options specified.");
        if (collection.Length is 1)
            return FromInvalid("No options specified.");
        var fields = new List<(object Name, object Value)>();

        foreach (var (_, index) in collection.WithIndex().Skip(1))
        {
            fields.Add((emojis[index - 1], collection[index]));
        }

        return FromFields(fields);
    }
        

    public static PollInfo FromInvalid(string reason)
        => new()
        {
            Validation = (false, reason)
        };

    public PollInfo WithPrompt(string prompt)
    {
        Prompt = prompt;
        return this;
    }

    public string Prompt { get; set; }
    public Dictionary<string, object> Fields { get; } = new();
    public (bool IsValid, string InvalidationReason) Validation { get; set; } = (true, null);
    public const string Footer = "Click one of the numbers below to vote.";

    public PollInfo AddFields(IEnumerable<(object Name, object Value)> fields)
    {
        foreach (var (name, value) in fields.Select(static x => (x.Name.ToString(), x.Value.ToString())))
            if (!(name.IsNullOrEmpty() || value is null)) Fields.Add(name, value);
            
        return this;
    }
}