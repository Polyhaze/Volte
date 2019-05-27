using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Tag")]
        [Priority(0)]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("Usage: |prefix|tag {name}")]
        public async Task TagAsync([Remainder] string name)
        {
            var data = Db.GetData(Context.Guild);
            var tag = data.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag is null)
            {
                await Context.CreateEmbed($"The tag **{name}** doesn't exist in this guild.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var response = tag.SanitizeContent()
                .Replace("{ServerName}", Context.Guild.Name)
                .Replace("{UserName}", Context.User.Username)
                .Replace("{UserMention}", Context.User.Mention)
                .Replace("{OwnerMention}", (await Context.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", Context.User.Discriminator);

            await Context.ReplyAsync(response);

            tag.Uses += 1;
            Db.UpdateData(data);
        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("Usage: |prefix|tagstats {name}")]
        public async Task TagStatsAsync([Remainder] string name)
        {
            var data = Db.GetData(Context.Guild);
            var tag = data.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag is null)
            {
                await Context.CreateEmbed($"The tag **{name}** doesn't exist in this guild.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var u = await Context.Client.Rest.GetUserAsync(tag.CreatorId);

            await Context.CreateEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", $"{u}", true)
                .AddField("Uses", $"**{tag.Uses}**", true)
                .SendToAsync(Context.Channel);
        }

        [Command("Tags")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("Usage: |prefix|tags")]
        public async Task TagsAsync()
        {
            var data = Db.GetData(Context.Guild);
            await Context.CreateEmbedBuilder(
                    data.Extras.Tags.Count == 0
                        ? "None"
                        : $"`{data.Extras.Tags.Select(x => x.Name).Join("`, `")}`"
                ).WithTitle($"Available Tags for {Context.Guild.Name}")
                .SendToAsync(Context.Channel);
        }
    }
}