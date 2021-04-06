using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Urban", "Definition")]
        [Description("Brings up the first result from Urban Dictionary's API for a word.")]
        public async Task<ActionResult> UrbanAsync([Remainder, Description("The word to get a definition for.")]
            string word)
        {
            var res = await RequestUrbanDefinitionsAsync(word);

            EmbedBuilder CreateEmbed(UrbanEntry entry) => Context.CreateEmbedBuilder()
                .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/vi/7/70/Urban_Dictionary_logo.png")
                .AddField("Word", Format.Bold(entry.Word) ?? "<error occurred>", true)
                .AddField("Thumbs Up/Down", $"{entry.Upvotes}/{entry.Downvotes}", true)
                .AddField("Definition", entry.Definition.IsNullOrEmpty() ? "No definition provided?" : entry.Definition)
                .AddField("Example", entry.Example.IsNullOrEmpty() ? "None provided" : entry.Example)
                .AddField("Author", entry.Author.IsNullOrEmpty() ? "None provided" : entry.Author, true)
                .AddField("URL", entry.Permalink.IsNullOrEmpty() ? "None provided" : entry.Permalink, true)
                .WithFooter($"Created {entry.CreatedAt.FormatPrettyString()}");


            var pages = res.Select(CreateEmbed).ToList();

            return pages.IsEmpty() 
                ? BadRequest("That word didn't have a definition of Urban Dictionary.")
                : pages.Count is 1 
                    ? Ok(pages.First()) 
                    : Ok(pages);
        }
    }
}