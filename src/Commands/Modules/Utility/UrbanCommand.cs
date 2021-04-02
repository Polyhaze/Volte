using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Urban", "Definition")]
        [Description("Brings up the first result from Urban Dictionary's API for a word.")]
        public async Task<ActionResult> UrbanAsync([Remainder, Description("The word to get a definition for.")] string word)
        {
            var res = await GetDefinitionAsync(HttpUtility.UrlEncode(word));
            if (res is null)
                return BadRequest("Something went wrong. Try again later.");
            
            var relevant = res.Entries.FirstOrDefault();
            if (relevant is null)
                return BadRequest("That word didn't have a definition on urban dictionary.");
            
            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/vi/7/70/Urban_Dictionary_logo.png")
                .AddField("Word", relevant.Word, true)
                .AddField("Score", relevant.Score, true)
                .AddField("Definition", relevant.Definition)
                .AddField("Example", relevant.Example)
                .AddField("Author", relevant.Author, true)
                .AddField("URL", relevant.Permalink, true)
                .WithFooter($"Created {relevant.CreatedAt.FormatPrettyString()}"));
        }
    }
}