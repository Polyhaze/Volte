using System.Web;
using Volte.Systems.Scripting;
using Volte.Systems.Interactive;

namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule : VolteModule
{
    public MessageService MessageService { get; set; }

    public static readonly Dictionary<string[], string> ZalgoNamedArguments = new()
    {
        { ["max"], "Use the highest intensity and include every character type. This option takes no value." },
        { ["up"], "Include upwards characters. This option takes no value." },
        { ["mid", "middle"], "Include middle characters. This option takes no value." },
        { ["down"], "Include downward characters. This option takes no value." },
        { ["intensity"], "`high`, `med`, or `low`" }
    };

    private readonly Dictionary<char, string> _nato = new()
    {
        {'a', "Alfa"}, {'b', "Bravo"}, 
        {'c', "Charlie"}, {'d', "Delta"},
        {'e', "Echo"}, {'f', "Foxtrot"}, 
        {'g', "Golf"}, {'h', "Hotel"},
        {'i', "India"}, {'j', "Juliett"}, 
        {'k', "Kilo"}, {'l', "Lima"},
        {'m', "Mike"}, {'n', "November"}, 
        {'o', "Oscar"}, {'p', "Papa"},
        {'q', "Quebec"}, {'r', "Romeo"}, 
        {'s', "Sierra"}, {'t', "Tango"},
        {'u', "Uniform"}, {'v', "Victor"}, 
        {'w', "Whiskey"}, {'x', "X-ray"},
        {'y', "Yankee"}, {'z', "Zulu"},
            
        {'1', "One"}, {'2', "Two"},
        {'3', "Three"}, {'4', "Four"}, 
        {'5', "Five"}, {'6', "Six"},
        {'7', "Seven"}, {'8', "Eight"}, 
        {'9', "Nine"}, {'0', "Zero"}
    };

    private string GetNato(char i) =>
        _nato.TryGetValue(i, out var nato) ? nato : throw new ArgumentOutOfRangeException(i.ToString());

    private (
        IOrderedEnumerable<(string Name, bool Value)> Allowed,
        IOrderedEnumerable<(string Name, bool Value)> Disallowed
        ) GetPermissions(
            IGuildUser user)
    {
        var propDict = user.GuildPermissions.GetType().GetProperties()
            .Where(static a => a.PropertyType.Inherits<bool>())
            .Select(a => (a.Name.Humanize(LetterCasing.Title), a.GetValue(user.GuildPermissions).Cast<bool>()))
            .OrderByDescending(static ab => ab.Item2 ? 1 : 0)
            .ToList(); //holy reflection

        return (propDict.Where(static ab => ab.Item2).OrderBy(static a => a.Item1.Length),
            propDict.Where(static ab => !ab.Item2).OrderBy(static a => a.Item1.Length));
    }
}

[RequireGuildAdmin]
public sealed partial class AdminUtilityModule : VolteModule
{
    public HttpClient Http { get; set; }

    public static string[] AllowedPasteSites { get; set; }

    public static readonly Dictionary<string[], string> AnnounceNamedArguments = new()
    {
        { ["ping", "mention"], "everyone, here, or a role ID" },
        { ["foot", "footer"], "Set the embed's footer content." },
        { ["thumbnail"], "Set the embed's small thumbnail URL." },
        { ["image"], "Set the embed's large image URL." },
        { ["title"], "Set the embed's title content." },
        { ["color"], "Set the embed's color." },
        {
            ["crosspost", "publish"],
            "Whether or not to automatically publish this message if it's sent in an announcement channel. This option takes no value."
        },
        {
            ["keepmsg", "keepmessage"],
            "Prevents deletion of the command message allowing people to see what input you used. Useful for demonstrations. This option takes no value."
        },
        {
            ["desc", "description"],
            "Set the embed's description content."
            /* If this is a URL to a raw paste on any known website; the embed's description will be that paste's content.
             {Format.Url("Supported sites.", "https://paste.greemdev.net/volteAllowedPasteSites")}"*/
        },
        {
            ["author"],
            "Set the author of the embed. `self` or `me` will make you the author; `bot`, `you`, or `volte` will make volte the author, or you can use a server member's ID."
        }
    };

    /// <summary>
    ///     Sends an HTTP <see cref="HttpMethod.Get"/> request to Urban Dictionary's public API requesting the definitions of <paramref name="word"/>.
    /// </summary>
    /// <param name="word">The word/phrase to search for. This method URL encodes it.</param>
    /// <returns><see cref="IEnumerable{UrbanEntry}"/> if the request was successful; <see langword="null"/> otherwise.</returns>
    public async Task<IReadOnlyList<UrbanEntry>> RequestUrbanDefinitionsAsync(string word)
    {
        var get = await Http.GetAsync(
            $"https://api.urbandictionary.com/v0/define?term={HttpUtility.UrlEncode(word)}".Trim(),
            HttpCompletionOption.ResponseContentRead);

        get.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<UrbanApiResponse>(await get.Content.ReadAsStringAsync()).Entries;
    }
}

[RequireBotOwner]
public sealed partial class BotOwnerModule : VolteModule
{
    public HttpClient Http { get; set; }
    public CancellationTokenSource Cts { get; set; }
    public AddonService Addon { get; set; }
}

[RequireGuildModerator]
public sealed partial class ModerationModule : VolteModule
{
    public static string GetReason(string action, IUser moderator, string reason) =>
        reason is null
            ? GetDefaultReason(action, moderator)
            : $"{moderator.Username} ({moderator.Id}): {reason}";

    public static string GetDefaultReason(string action, IUser moderator)
        => $"{action} by {moderator.Username} ({moderator.Id})";

    public InteractiveService Interactive { get; set; }

    public static readonly Dictionary<string[], string> UnixBanNamedArguments = new()
    {
        {
            ["deleteDays", "days"],
            "The amount of days of messages to delete. Note that Discord is strict with this value, and it can only be 0-7."
        },
        {
            ["reason"], "The reason for the ban. If this is not provided, the reason will be \"Banned by a Moderator.\""
        },
        {
            ["shadow"],
            "If this option is present, the banned member will not be made aware of who actually banned them via the embed author. This option takes no value."
        },
        {
            ["soft", "softly"],
            "Immediately unbans the member after a ban, AKA \"softban.\" This option takes no value."
        }
    };
}

[Group("Settings", "Setting", "Options", "Option")]
[Description("The set of commands used to modify how Volte functions in your guild.")]
[RequireGuildAdmin]
public partial class SettingsModule : VolteModule
{
    [Command, DummyCommand, Description("The set of commands used to modify how Volte functions in your guild.")]
    public async Task<ActionResult> BaseAsync()
        => Ok(await TextCommandHelper.CreateCommandEmbedAsync(Context.Command, Context));
}