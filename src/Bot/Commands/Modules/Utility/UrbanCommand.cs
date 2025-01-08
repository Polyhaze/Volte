using Volte.Interactive;

namespace Volte.Commands.Text.Modules;

public partial class UtilityModule
{
    [Command("Urban", "Definition")]
    [Description("Brings up the first result from Urban Dictionary's API for a word.")]
    public async Task<ActionResult> UrbanAsync(
        [Remainder, Description("The word to get a definition for.")] string word)
    {
        var res = await RequestUrbanDefinitionsAsync(word);
        var pages = res.Select(createEmbed).ToList();

        return pages.Count switch
        {
            0 => BadRequest("That word didn't have a definition of Urban Dictionary."),
            1 => Ok(pages.First()),
            _ => Ok(new PaginatedMessage.Builder()
                .WithPages(pages)
                .WithTitle(word)
                .WithDefaults(Context))
        };

        EmbedBuilder createEmbed(UrbanEntry entry)
        {
            if (entry.Definition.Length > 1024)
                entry.Definition = entry.Definition
                    .Truncate(980, removedChars =>
                        Format.Bold(
                            $"\n...and {removedChars} more {"character".ToQuantity(removedChars, ShowQuantityAs.None)}.")
                    );
            else if (entry.Definition.IsNullOrEmpty())
                entry.Definition = "<error occurred>";

            return Context.CreateEmbedBuilder()
                .WithThumbnailUrl(
                    "https://raw.githubusercontent.com/GreemDev/VolteAssets/main/Urban_Dictionary_logo.png")
                .AddField("URL", entry.Permalink.IsNullOrEmpty() ? "None provided" : entry.Permalink, true)
                .AddField("Thumbs Up/Down", $"{entry.Upvotes}/{entry.Downvotes}", true)
                .AddField("Score", entry.Score, true)
                .AddField("Definition", entry.Definition)
                .AddField("Example", entry.Example.IsNullOrEmpty() ? "None provided" : entry.Example)
                .AddField("Author", entry.Author.IsNullOrEmpty() ? "None provided" : entry.Author, true)
                .WithFooter($"Created {entry.CreatedAt.FormatPrettyString()}");
        }
    }
}