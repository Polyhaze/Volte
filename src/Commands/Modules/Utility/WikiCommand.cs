using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        private const string BaseWikiUrl = "https://github.com/Ultz/Volte/wiki";

        private readonly Dictionary<string, Uri> _wikiPageUris = new Dictionary<string, Uri>
        {
            {"Home", new Uri(BaseWikiUrl)},
            {"Features", new Uri($"{BaseWikiUrl}/Features")},
            {"Contributing", new Uri($"{BaseWikiUrl}/Contributing")},
            {"Setting Volte Up", new Uri($"{BaseWikiUrl}/Setting-Volte-Up")},
            {"Argument Cheatsheet", new Uri($"{BaseWikiUrl}/Argument-Cheatsheet")},
            {"Developers:Selfhost:Windows", new Uri($"{BaseWikiUrl}/Windows")},
            {"Developers:Selfhost:Linux", new Uri($"{BaseWikiUrl}/Linux")},
            {"Developers:Dependency Injection", new Uri($"{BaseWikiUrl}/Dependency-Injection")}
        };

        [Command("Wiki", "VolteWiki")]
        [Description("List all wiki pages or get a specific one in this one command.")]
        public Task<ActionResult> WikiAsync(
            [Remainder,
             Description("The wiki page you want to see. If none is provided, it will display a list of all pages.")]
            string page = null)
        {
            var embed = Context.CreateEmbedBuilder()
                .WithThumbnailUrl("https://img.greemdev.net/YmdTzPoEYx/volte_whiteorangepurple.png");

            if (page is null)
                return Ok(_wikiPageUris.Select(x => embed.WithTitle(x.Key).WithDescription(x.Value.ToString())));

            return Ok(embed.WithDescription(_wikiPageUris.TryGetValue(page, out var uri)
                ? Format.Url(_wikiPageUris.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(page)), uri.ToString())
                : $"{page} wasn't found. Here's a list of valid wiki pages: {FormatPages()}"));


            string FormatPages() => new StringBuilder().Apply(sb =>
            {
                foreach (var (key, value) in _wikiPageUris)
                    sb.AppendLine(Format.Url(key, value.ToString()));
            }).ToString();
        }
    }
}