using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Tag")]
        [Priority(0)]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("tag {Tag}")]
        public Task<ActionResult> TagAsync([Remainder] Tag tag)
        {
            tag.Uses += 1;
            Db.UpdateData(Context.GuildData);

            if (Context.GuildData.Configuration.EmbedTagsAndShowAuthor)
            {
                return Ok(Context.CreateEmbedBuilder(tag.FormatContent(Context)).WithAuthor(author: null).WithFooter($"Requested by {Context.User}."), async message =>
                {
                    if (Context.GuildData.Configuration.DeleteMessageOnTagCommandInvocation)
                    {
                        await Context.Message.TryDeleteAsync();
                    }
                });
            }

            return Ok(tag.FormatContent(Context), async message =>
            {
                if (Context.GuildData.Configuration.DeleteMessageOnTagCommandInvocation)
                {
                    await Context.Message.TryDeleteAsync();
                }
            }, false);

        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("tagstats {Tag}")]
        public async Task<ActionResult> TagStatsAsync([Remainder] Tag tag)
        {
            var u = await Context.Client.GetShardFor(Context.Guild).Rest.GetUserAsync(tag.CreatorId);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", $"{u}", true)
                .AddField("Uses", $"**{tag.Uses}**", true));
        }

        [Command("Tags")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("tags")]
        public Task<ActionResult> TagsAsync()
        {
            var tagsList = Context.GuildData.Extras.Tags;
            if (tagsList.IsEmpty()) return BadRequest("This guild doesn't have any tags.");
            else
            {
                var list = tagsList.Select(x => $"`{x.Name}`").ToList();
                var pages = new List<string>();

                do
                {
                    pages.Add(list.Take(10).Join("\n"));
                    list.RemoveRange(0, list.Count < 10 ? list.Count : 10);
                } while (!list.IsEmpty());
                
                return Ok(async () =>
                {
                    await PagedReplyAsync(new PaginatedMessage
                    {
                        Author = Context.User,
                        Pages = pages
                    });
                }, false);
            }
        }
            
    }
}