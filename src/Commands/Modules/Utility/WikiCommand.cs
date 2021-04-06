using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        private readonly string _baseWikiUrl = "https://github.com/Ultz/Volte/wiki";

        [Command("Wiki", "VolteWiki")]
        [Description("List all wiki pages or get a specific one in this one command.")]
        public Task<ActionResult> WikiAsync([Remainder, Description("The wiki page you want to see. If none is provided, it will display a list of all pages.")] string page = null)
        {
            var embed = Context.CreateEmbedBuilder().WithThumbnailUrl("https://i.greemdev.net/volte_whitepinkyellow.png");
            var pages = new Dictionary<string, string>
            { 
                { "Home", _baseWikiUrl },
                { "Features", $"{_baseWikiUrl}/Features"},
                { "Contributing", $"{_baseWikiUrl}/Contributing"},
                { "Setting Volte Up", $"{_baseWikiUrl}/Setting-Volte-Up"},
                { "Argument Cheatsheet", $"{_baseWikiUrl}/Argument-Cheatsheet"},
                { "Developers:Selfhost:Windows", $"{_baseWikiUrl}/Windows"},
                { "Developers:Selfhost:Linux", $"{_baseWikiUrl}/Linux"},
                { "Developers:Dependency Injection", $"{_baseWikiUrl}/Dependency-Injection"}
            };

            if (page is null)
                return Ok(embed.WithDescription(FormatPages()));

            return Ok(embed.WithDescription(pages.ContainsKey(page)
                ? $"[{pages.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(page))}]({pages.FirstOrDefault(x => x.Key.EqualsIgnoreCase(page)).Value})"
                : $"{page} wasn't found. Here's a list of valid wiki pages: {FormatPages()}"));


            string FormatPages()
            {
                var formattedPages = new StringBuilder();
                foreach (var (key, value) in pages)
                    formattedPages.AppendLine($"[{key}]({value})");

                return formattedPages.ToString();
            }
        }
    }
}
