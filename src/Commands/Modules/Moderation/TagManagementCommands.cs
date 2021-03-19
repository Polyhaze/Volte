using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("TagCreate", "TagAdd", "TagNew")]
        [Priority(1)]
        [Description("Creates a tag with the specified name and response (in that order).")]
        [Remarks("tagcreate {String} {String}")]
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

            Context.GuildData.Extras.Tags.Add(tag);
            Db.Save(Context.GuildData);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Tag Created!")
                .AddField("Name", tag.Name)
                .AddField("Response", tag.Response.Length > EmbedFieldBuilder.MaxFieldValueLength ? "<Cannot display; too large.>" : tag.Response)
                .AddField("Creator", Context.User.Mention));
        }

        [Command("TagEdit", "TagEd", "TagE")]
        [Priority(1)]
        [Description("Edit a tag's content if it exists.")]
        [Remarks("tagedit {Tag} {String}")]
        [RequireGuildModerator]
        public Task<ActionResult> TagEditAsync(Tag tag, [Remainder] string response)
        {
            Context.GuildData.Extras.Tags.Remove(tag);
            tag.Response = response;
            Context.GuildData.Extras.Tags.Add(tag);
            Db.Save(Context.GuildData);
            return Ok($"Successfully modified the content of tag **{tag.Name}**.");
        }

        [Command("TagDelete", "TagDel", "TagRem")]
        [Priority(1)]
        [Description("Deletes a tag if it exists.")]
        [Remarks("tagdelete {Tag}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagDeleteAsync([Remainder]Tag tag)
        {
            Context.GuildData.Extras.Tags.Remove(tag);
            Db.Save(Context.GuildData);
            return Ok($"Deleted the tag **{tag.Name}**, created by " +
                      $"**{await Context.Client.Rest.GetUserAsync(tag.CreatorId)}**, with " +
                      $"**{"use".ToQuantity(tag.Uses)}**.");
        }
    }
}