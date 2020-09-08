using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    [Group("Tags", "T", "Tag")]
    public sealed class TagsModule : VolteModule 
    {
        [Command]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("tags {Tag}")]
        public Task<ActionResult> TagAsync([Remainder, RequiredArgument] Tag tag)
        {
            tag.Uses += 1;
            Db.UpdateData(Context.GuildData);

            if (Context.GuildData.Configuration.EmbedTagsAndShowAuthor)
            {
                return Ok(Context.CreateEmbedBuilder(tag.FormatContent(Context)), async message =>
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
        
        [Command("Embed", "Em")]
        [Description(
            "Toggles whether or not Tags requested in your guild will be in an embed and be shown with the person who requested the Tag.")]
        [Remarks("tag embed {Boolean}")]
        public Task<ActionResult> ShowRequesterAndEmbedTagsAsync([RequiredArgument] bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.EmbedTagsAndShowAuthor = enabled;
                return data;
            });
            return Ok(enabled
                ? "Tags will now show their requester and be displayed in an embed!"
                : "Tags will **NO LONGER** show their requester and be displayed in an embed!");
        }

        [Command("Stats")]
        [Description("Shows stats for a tag.")]
        [Remarks("tags stats {Tag}")]
        public async Task<ActionResult> TagStatsAsync([Remainder, RequiredArgument] Tag tag)
        {
            var u = await Context.Client.GetShardFor(Context.Guild).GetUserAsync(tag.CreatorId);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Tag Stats for {tag.Name}")
                .AddField("Response", $"`{tag.Response}`")
                .AddField("Creator", $"{u.AsPrettyString()}")
                .AddField("Uses", $"**{tag.Uses}**"));
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
                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        tagsList.Select(x => $"`{x.Name}`").GetPages(10));
                }, false);
            }
        }

        [Command("Create", "Add", "New")]
        [Description("Creates a tag with the specified name and response (in that order).")]
        [Remarks("tags create {String} {String}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagCreateAsync([RequiredArgument] string name, [Remainder, RequiredArgument] string response)
        {
            var tag = Context.GuildData.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag is not null)
            {
                var user = await Context.Client.GetShardFor(Context.Guild).GetUserAsync(tag.CreatorId);
                return BadRequest(
                    $"Cannot make the tag **{tag.Name}**, as it already exists and is owned by **{user.AsPrettyString()}**.");
            }

            tag = new Tag
            {
                Name = name,
                Response = response,
                CreatorId = Context.Member.Id,
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
                .AddField("Creator", Context.Member.Mention));
        }

        [Command("Delete", "Del", "Rem")]
        [Description("Deletes a tag if it exists.")]
        [Remarks("tags delete {Tag}")]
        [RequireGuildModerator]
        public async Task<ActionResult> TagDeleteAsync([Remainder, RequiredArgument] Tag tag)
        {
            ModifyData(data =>
            {
                data.Extras.Tags.RemoveAt(data.Extras.Tags.FindIndex(x => x.Name == tag.Name && x.CreatorId == tag.CreatorId));
                return data;
            });
            return Ok($"Deleted the tag **{tag.Name}**, created by " +
                      $"**{(await Context.Client.GetShardFor(Context.Guild).GetUserAsync(tag.CreatorId)).AsPrettyString()}**, with " +
                      $"**{"use".ToQuantity(tag.Uses)}**.");
        }
        
        
        [Command("Edit")]
        [Description("Edits a tag's content if it exists.")]
        [Remarks("tags edit {Tag} {String}")]
        [RequireGuildModerator]
        public Task<ActionResult> TagEditAsync([RequiredArgument] Tag tag, [Remainder, RequiredArgument] string content)
        {
            tag.Response = content;
            ModifyData(data =>
            {
                data.Extras.Tags.RemoveAt(data.Extras.Tags.FindIndex(x => x.Name == tag.Name && x.CreatorId == tag.CreatorId));
                data.Extras.Tags.Add(tag);
                return data;
            });
            
            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Tag Updated")
                .AddField("Name", tag.Name)
                .AddField("New Response", content)
                .AddField("Creator", Context.Member.AsPrettyString())
                .AddField("Uses", tag.Uses));
        }
    }
}