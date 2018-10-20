using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    public class AutoroleCommand : SIVACommand {
        [Command("Autorole")]
        public async Task Autorole([Remainder]string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.Message, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            var roletoApply = Context.Guild.Roles
                .FirstOrDefault(r => r.Name.ToLower().Equals(role.ToLower()));
            if (roletoApply == null) {
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"The specified role, **{role}**, doesn't exist on this server."));
                return;
            }

            config.Autorole = roletoApply.Name;
            ServerConfig.Save();
            await Reply(Context.Channel, CreateEmbed(Context,
                $"Successfully set **{roletoApply.Name}** as the Autorole for this server."));
        }
    }
}