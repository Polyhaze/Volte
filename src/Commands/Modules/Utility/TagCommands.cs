using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed partial class TagsModule
    {
        [Command("Get")]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("tags get {Tag}")]
        public Task<ActionResult> TagAsync([Remainder] Tag tag)
        {
            tag.Uses += 1;
            Db.UpdateData(Context.GuildData);

            if (Context.GuildData.Configuration.EmbedTagsAndShowAuthor)
            {
                return Ok(Context.CreateEmbedBuilder(tag.FormatContent(Context)).WithFooter($"Requested by {Context.Member}."), async message =>
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

        [Command("Stats")]
        [Description("Shows stats for a tag.")]
        [Remarks("tags stats {Tag}")]
        public async Task<ActionResult> TagStatsAsync([Remainder] Tag tag)
        {
            var u = await Context.Client.GetShardFor(Context.Guild).GetUserAsync(tag.CreatorId);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", $"{u}", true)
                .AddField("Uses", $"**{tag.Uses}**", true));
        }

        [Command("List")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("tags list")]
        public Task<ActionResult> TagsAsync()
        {
            var tagsList = Context.GuildData.Extras.Tags;
            if (tagsList.IsEmpty()) return BadRequest("This guild doesn't have any tags.");
            else
            {
                return Ok(new PaginatedMessageBuilder(Context)
                    .WithPages(tagsList.Select(x => $"`{x.Name}`"))
                    .SplitPages(10)
                    .Build());
            }
        }
            
    }
}