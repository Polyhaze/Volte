using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Tag")]
        [Priority(0)]
        [Description("Gets a tag's contents if it exists.")]
        [Remarks("Usage: |prefix|tag {name}")]
        public Task<VolteCommandResult> TagAsync([Remainder] string name)
        {
            var data = Db.GetData(Context.Guild);
            var tag = data.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag is null)
                return BadRequest($"The tag **{name}** doesn't exist in this guild.");

            tag.Uses += 1;
            Db.UpdateData(data);

            return Ok(tag.SanitizeContent()
                .Replace("{ServerName}", Context.Guild.Name)
                .Replace("{UserName}", Context.User.Username)
                .Replace("{UserMention}", Context.User.Mention)
                .Replace("{OwnerMention}", Context.Guild.Owner.Mention)
                .Replace("{UserTag}", Context.User.Discriminator), null, false);
        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("Usage: |prefix|tagstats {name}")]
        public async Task<VolteCommandResult> TagStatsAsync([Remainder] string name)
        {
            var data = Db.GetData(Context.Guild);
            var tag = data.Extras.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag is null)
                return BadRequest($"The tag **{name}** doesn't exist in this guild.");

            var u = await Context.Client.GetShardFor(Context.Guild).Rest.GetUserAsync(tag.CreatorId);

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", $"{u}", true)
                .AddField("Uses", $"**{tag.Uses}**", true));
        }

        [Command("Tags")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("Usage: |prefix|tags")]
        public Task<VolteCommandResult> TagsAsync()
        {
            var data = Db.GetData(Context.Guild);
            return Ok(Context.CreateEmbedBuilder(
                data.Extras.Tags.Count == 0
                    ? "None"
                    : $"`{data.Extras.Tags.Select(x => x.Name).Join("`, `")}`"
            ).WithTitle($"Available Tags for {Context.Guild.Name}"));
        }
    }
}