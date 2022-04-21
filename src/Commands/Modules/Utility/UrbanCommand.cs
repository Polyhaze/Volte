using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Entities;
using Volte.Interactive;

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

            EmbedBuilder CreateEmbed(UrbanEntry entry)
            {
                if (entry.Definition.Length > 1024)
                {
                    var oldDefLeng = entry.Definition.Length;
                    entry.Definition = string.Join(string.Empty, entry.Definition.Take(980)) +
                                       Format.Bold(
                                           $"\n...and {oldDefLeng - 980} more {"character".ToQuantity(oldDefLeng - 980).Split(" ").Last()}.");
                }
                else if (entry.Definition.IsEmpty())
                    entry.Definition = "<error occurred>";

                return Context.CreateEmbedBuilder()
                    .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/vi/7/70/Urban_Dictionary_logo.png")
                    .AddField("URL", entry.Permalink.IsNullOrEmpty() ? "None provided" : entry.Permalink, true)
                    .AddField("Thumbs Up/Down", $"{entry.Upvotes}/{entry.Downvotes}", true)
                    .AddField("Score", entry.Score, true)
                    .AddField("Definition", entry.Definition)
                    .AddField("Example", entry.Example.IsNullOrEmpty() ? "None provided" : entry.Example)
                    .AddField("Author", entry.Author.IsNullOrEmpty() ? "None provided" : entry.Author, true)
                    .WithFooter($"Created {entry.CreatedAt.FormatPrettyString()}");
            }


            var pages = res.Select(CreateEmbed).ToList();

            return pages.IsEmpty()
                ? BadRequest("That word didn't have a definition of Urban Dictionary.")
                : pages.Count is 1
                    ? Ok(pages.First())
                    : Ok(PaginatedMessage.NewBuilder()
                        .WithPages(pages)
                        .WithTitle(word)
                        .WithDefaults(Context));
        }
    }
}