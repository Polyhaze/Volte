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

            var relevant = res?.Entries?.FirstOrDefault();

            return res is null
                ? BadRequest("Something went wrong. Try again later.")
                : relevant is null
                    ? BadRequest("That word didn't have a definition on urban dictionary.")
                    : Ok(Context.CreateEmbedBuilder()
                        .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/vi/7/70/Urban_Dictionary_logo.png")
                        .AddField("Word", Format.Bold(relevant.Word), true)
                        .AddField("Thumbs Up/Down", $"{relevant.Upvotes}/{relevant.Downvotes}", true)
                        .AddField("Definition", relevant.Definition)
                        .AddField("Example", relevant.Example)
                        .AddField("Author", relevant.Author, true)
                        .AddField("URL", relevant.Permalink, true)
                        .WithFooter($"Created {relevant.CreatedAt.FormatPrettyString()}"));
        }
    }
}