using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Tag"), Priority(0)]
        [Summary("Gets a tag's contents if it exists.")]
        [Remarks("Usage: |prefix|tag {name}")]
        public async Task Tag([Remainder] string name) {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag == null) {
                await Context.CreateEmbed($"The tag **{name}** doesn't exist in this guild.")
                    .SendTo(Context.Channel);
                return;
            }

            var response = tag.Response
                .Replace("{ServerName}", Context.Guild.Name)
                .Replace("{UserName}", Context.User.Username)
                .Replace("{UserMention}", Context.User.Mention)
                .Replace("{OwnerMention}", (await Context.Guild.GetOwnerAsync()).Mention)
                .Replace("{UserTag}", Context.User.Discriminator);

            await Context.CreateEmbed(response).SendTo(Context.Channel);

            tag.Uses += 1;
            Db.UpdateConfig(config);
        }

        [Command("Tag Stats"), Priority(1)]
        [Summary("Shows stats for a tag.")]
        [Remarks("Usage: |prefix|tag stats {name}")]
        public async Task TagStats([Remainder]string name) {
            var config = Db.GetConfig(Context.Guild);
            var tag = config.Tags.FirstOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (tag == null) {
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