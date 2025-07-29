namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    private const string BaseWikiUrl = "https://github.com/Polyhaze/Volte/wiki";

    private readonly Dictionary<string, Uri> _wikiPageUris = new()
    {
        {"Home", new Uri(BaseWikiUrl)},
        {"Features", new Uri($"{BaseWikiUrl}/Features")},
        {"Setting Volte Up", new Uri($"{BaseWikiUrl}/Setting-Volte-Up")},
        {"Argument Cheatsheet", new Uri($"{BaseWikiUrl}/Argument-Cheatsheet")},
        {"Selfhost:Windows", new Uri($"{BaseWikiUrl}/Windows")},
        {"Selfhost:Linux", new Uri($"{BaseWikiUrl}/Linux")},
        {"Developers:Contributing", new Uri($"{BaseWikiUrl}/Contributing")},
        {"Developers:Dependency Injection", new Uri($"{BaseWikiUrl}/Dependency-Injection")}
    };

    [Command("Wiki", "VolteWiki")]
    [Description("List all wiki pages or get a specific one in this one command.")]
    public Task<ActionResult> WikiAsync(
        [Remainder,
         Description("The wiki page you want to see. If none is provided, it will display a list of all pages.")]
        string page = null)
    {
        if (page is null)
            return Ok(FormatPages().JoinToString('\n'));
        
        var embed = Context.CreateEmbedBuilder()
            .WithThumbnailUrl("https://raw.githubusercontent.com/GreemDev/VolteAssets/main/volte_whiteorangepurple.png");

        return Ok(embed.WithDescription(_wikiPageUris.TryGetValue(page, out var uri)
            ? Format.Url(_wikiPageUris.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(page)), uri.ToString())
            : $"{page} wasn't found. Here's a list of valid wiki pages: {FormatPages().JoinToString('\n')}"));


        IEnumerable<string> FormatPages()
        {
            foreach (var (key, value) in _wikiPageUris)
                yield return Format.Url(key, value.ToString());
        }
    }
}