using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core.Models.Guild;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class TagsModule
    {
        [Command("Create", "Add", "New")]
        [Description("Creates a tag with the specified name and response (in that order).")]
        [Remarks("tags create {String} {String}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagCreateAsync(string name, [Remainder] string response)
        {
            var tag = Context.GuildData.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag != null)
            {
                var user = await Context.Client.GetShardFor(Context.Guild).Rest.GetUserAsync(tag.CreatorId);
                return BadRequest(
                    $"Cannot make the tag **{tag.Name}**, as it already exists and is owned by **{user}**.");
            }

            tag = new Tag
            {
                Name = name,
                Response = response,
                CreatorId = Context.User.Id,
                GuildId = Context.Guild.Id,
                Uses = default
            };

            ModifyData(data =>
            {
                data.Extras.Tags.Add(tag);
                return data;
            });

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Tag Created!")
                .AddField("Name", tag.Name)
                .AddField("Response", tag.Response)
                .AddField("Creator", Context.User.Mention));
        }

        [Command("Delete", "Del", "Rem")]
        [Description("Deletes a tag if it exists.")]
        [Remarks("tags delete {Tag}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagDeleteAsync([Remainder]Tag tag)
        {
            ModifyData(data =>
            {
                data.Extras.Tags.Remove(tag);
                return data;
            });
            return Ok($"Deleted the tag **{tag.Name}**, created by " +
                      $"**{await Context.Client.Shards.First().Rest.GetUserAsync(tag.CreatorId)}**, with " +
                      $"**{"use".ToQuantity(tag.Uses)}**.");
        }
    }
}