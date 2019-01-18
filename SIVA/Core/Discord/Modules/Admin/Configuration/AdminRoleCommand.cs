using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class AdminRoleCommand : SIVACommand {
        [Command("AdminRole")]
        public async Task AdminRole([Remainder] string roleName) {
            var embed = CreateEmbed(Context, "").ToEmbedBuilder();
            if (!UserUtils.IsServerOwner(Context.User, Context.Guild)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            if (Context.Guild.Roles.Any(r => r.Name.ToLower().Equals(roleName.ToLower()))) {
                var role = Context.Guild.Roles.First(r => r.Name == roleName);
                config.AdminRole = role.Id;
                ServerConfig.Save();
                embed.WithDescription($"Set **{role.Name}** as the Admin role for this server.");
            }
            else {
                embed.WithDescription($"**{roleName}** doesn't exist in this server.");
            }

            await Reply(Context.Channel, embed.Build());
        }
    }
}