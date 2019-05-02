using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Extensions;
using Gommon;

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
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag == null)
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
            Db.UpdateConfig(config);
        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("Usage: |prefix|tagstats {name}")]
        public async Task TagStatsAsync([Remainder] string name)
        {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag == null)
            {
                await Context.CreateEmbed($"The tag **{name}** doesn't exist in this guild.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var u = Context.Client.GetUser(tag.CreatorId);

            await Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", u is null ? $"{tag.CreatorId}" : $"{u.Mention}", true)
                .AddField("Uses", $"**{tag.Uses}**", true)
                .SendToAsync(Context.Channel);
        }

        [Command("Tags")]
        [Description("Lists all available tags in the current guild.")]
        [Remarks("Usage: |prefix|tags")]
        public async Task TagsAsync()
        {
            var config = Db.GetConfig(Context.Guild);
            await Context.CreateEmbedBuilder(
                config.Tags.Count == 0 
                ? "None" 
                : $"`{config.Tags.Select(x => x.Name).Join("`, `")}`"
                ).WithTitle($"Available Tags for {Context.Guild.Name}")
                .SendToAsync(Context.Channel);
        }

    }
}