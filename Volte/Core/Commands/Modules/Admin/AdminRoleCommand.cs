using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("Usage: |prefix|adminrole {roleName}")]
        [RequireGuildAdmin]
        public async Task AdminRole([Remainder] string roleName) {
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder();
            var config = Db.GetConfig(Context.Guild);
            if (Context.Guild.Roles.Any(r => r.Name.ToLower().Equals(roleName.ToLower()))) {
                var role = Context.Guild.Roles.First(r => r.Name == roleName);
                config.AdminRole = role.Id;
                Db.UpdateConfig(config);
                embed.WithDescription($"Set **{role.Name}** as the Admin role for this server.");
            }
            else {
                embed.WithDescription($"**{roleName}** doesn't exist in this server.");
            }

            await embed.SendTo(Context.Channel);
        }
    }
}