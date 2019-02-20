using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
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
                    .SendTo(Context.Channel);
                return;
            }

            var response = tag.SanitizeContent()
                .Replace("{ServerName}", Context.Guild.Name)
                .Replace("{UserName}", Context.User.Username)
                .Replace("{UserMention}", Context.User.Mention)
                .Replace("{OwnerMention}", Context.Guild.Owner.Mention)
                .Replace("{UserTag}", Context.User.Discriminator);

            await Context.ReplyAsync(response);

            tag.Uses += 1;
            Db.UpdateConfig(config);
        }

        [Command("TagStats")]
        [Priority(1)]
        [Description("Shows stats for a tag.")]
        [Remarks("Usage: |prefix|tagstats {name}")]
        public async Task TagStats([Remainder] string name)
        {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag == null)
            {
                await Context.CreateEmbed($"The tag **{name}** doesn't exist in this guild.")
                    .SendTo(Context.Channel);
                return;
            }

            var u = VolteBot.Client.GetUser(tag.CreatorId);

            await Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .WithTitle($"Tag {tag.Name}")
                .AddField("Response", $"`{tag.Response}`", true)
                .AddField("Creator", u == null ? $"{tag.CreatorId}" : $"{u.Mention}", true)
                .AddField("Uses", $"**{tag.Uses}**", true)
                .SendTo(Context.Channel);
        }
    }
}