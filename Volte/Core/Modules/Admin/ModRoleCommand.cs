using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("ModRole")]
        [Summary("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {roleName}")]
        public async Task ModRole([Remainder] string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            var embed = CreateEmbed(Context, string.Empty).ToEmbedBuilder();

            if (Context.Guild.Roles.Any(r => r.Name.ToLower() == roleName.ToLower())) {
                var role = Context.Guild.Roles.First(r => r.Name.ToLower() == roleName.ToLower());
                config.ModRole = role.Id;
                Db.UpdateConfig(config);
                embed.WithDescription($"Set **{role.Name}** as the Moderator role for this server.");
            }
            else {
                embed.WithDescription($"{roleName} doesn't exist in this server.");
            }

            await Context.Channel.SendMessageAsync(string.Empty, false, embed.Build());
        }
    }
}