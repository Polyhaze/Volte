using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Objects;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("TagCreate", "TagAdd", "TagNew")]
        [Priority(1)]
        [Description("Creates a tag with the specified name and response.")]
        [Remarks("Usage: |prefix|tagcreate {name} {response}")]
        [RequireGuildAdmin]
        public async Task TagCreateAsync(string name, [Remainder] string response)
        {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag != null)
            {
                var user = await Context.Client.GetUserAsync(tag.CreatorId);
                await Context
                    .CreateEmbed(
                        $"Cannot make the tag **{tag.Name}**, as it already exists and is owned by {user.Mention}.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var newTag = new Tag
            {
                Name = name,
                Response = response,
                CreatorId = Context.User.Id,
                GuildId = Context.Guild.Id,
                Uses = 0
            };

            config.Tags.Add(newTag);
            Db.UpdateConfig(config);

            await Context.CreateEmbedBuilder()
                .WithTitle("Tag Created!")
                .AddField("Name", newTag.Name)
                .AddField("Response", newTag.Response)
                .AddField("Creator", Context.User.Mention)
                .SendToAsync(Context.Channel);
        }

        [Command("TagDelete", "TagDel", "TagRem")]
        [Priority(1)]
        [Description("Deletes a tag if it exists.")]
        [Remarks("Usage: |prefix|tagdelete {name}")]
        public async Task TagDeleteAsync([Remainder] string name)
        {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));
            if (tag == null)
            {
                await Context.CreateEmbed($"Cannot delete the tag **{name}**, as it doesn't exist.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var user = await Context.Client.GetUserAsync(tag.CreatorId);

            config.Tags.Remove(tag);
            Db.UpdateConfig(config);
            await Context.CreateEmbed(
                    $"Deleted the tag **{tag.Name}**, created by " +
                    $"{(user != null ? user.Mention : $"user with ID **{tag.CreatorId}**")} with **{tag.Uses}** " +
                    $"{(tag.Uses != 1 ? "uses" : "use")}.")
                .SendToAsync(Context.Channel);
        }
    }
}