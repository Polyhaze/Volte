using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models.Guild;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("TagCreate", "TagAdd", "TagNew")]
        [Priority(1)]
        [Description("Creates a tag with the specified name and response.")]
        [Remarks("Usage: |prefix|tagcreate {name} {response}")]
        [RequireGuildModerator]
        public Task<ActionResult> TagCreateAsync(string name, [Remainder] string response)
        {
            var tag = Context.GuildData.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag != null)
            {
                var user = Context.Client.GetShardFor(Context.Guild).Rest.GetUserAsync(tag.CreatorId);
                return BadRequest(
                    $"Cannot make the tag **{tag.Name}**, as it already exists and is owned by **{user}**.");
            }

            var newTag = new Tag
            {
                Name = name,
                Response = response,
                CreatorId = Context.User.Id,
                GuildId = Context.Guild.Id,
                Uses = 0
            };

            Context.GuildData.Extras.Tags.Add(newTag);
            Db.UpdateData(Context.GuildData);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Tag Created!")
                .AddField("Name", newTag.Name)
                .AddField("Response", newTag.Response)
                .AddField("Creator", Context.User.Mention));
        }

        [Command("TagDelete", "TagDel", "TagRem")]
        [Priority(1)]
        [Description("Deletes a tag if it exists.")]
        [Remarks("Usage: |prefix|tagdelete {name}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagDeleteAsync([Remainder]Tag tag)
        {
            var user = await Context.Client.GetShardFor(Context.Guild).Rest.GetUserAsync(tag.CreatorId);

            Context.GuildData.Extras.Tags.Remove(tag);
            Db.UpdateData(Context.GuildData);
            return Ok($"Deleted the tag **{tag.Name}**, created by " +
                      $"**{user}**" +
                      $"{"use".ToQuantity(tag.Uses)}.");
        }
    }
}