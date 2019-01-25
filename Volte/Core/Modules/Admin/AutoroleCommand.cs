using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Autorole")]
        public async Task Autorole([Remainder]string role) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            var roletoApply = Context.Guild.Roles
                .FirstOrDefault(r => r.Name.ToLower().Equals(role.ToLower()));
            if (roletoApply == null) {
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"The specified role, **{role}**, doesn't exist on this server."));
                return;
            }

            config.Autorole = roletoApply.Name;
            Db.UpdateConfig(config);
            await Reply(Context.Channel, CreateEmbed(Context,
                $"Successfully set **{roletoApply.Name}** as the Autorole for this server."));
        }
    }
}