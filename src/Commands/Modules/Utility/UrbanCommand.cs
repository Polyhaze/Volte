using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Urban", "Definition")]
        [Description("Brings up the first result from Urban Dictionary's API for a word.")]
        public async Task<ActionResult> UrbanAsync([Remainder, Description("The word to get a definition for.")]
            string word)
        {
            var res = await GetDefinitionAsync(word);

            var pages = res?.Entries?.Select(entry => Context.CreateEmbedBuilder()
                    .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/vi/7/70/Urban_Dictionary_logo.png")
                    .AddField("Word", Format.Bold(entry.Word), true)
                    .AddField("Thumbs Up/Down", $"{entry.Upvotes}/{entry.Downvotes}", true)
                    .AddField("Definition", entry.Definition)
                    .AddField("Example", entry.Example)
                    .AddField("Author", entry.Author, true)
                    .AddField("URL", entry.Permalink, true)
                    .WithFooter($"Created {entry.CreatedAt.FormatPrettyString()}"))
                ?.ToList();

            if (pages is null || pages.IsEmpty())
                return BadRequest("That word didn't have a definition of Urban Dictionary.");

            if (pages.Count is 1)
                return Ok(pages[0]);
            
            return Ok(pages);
        }
    }
}