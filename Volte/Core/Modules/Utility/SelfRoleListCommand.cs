using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("SelfRoleList"), Alias("Srl")]
        [Summary("Gets a list of self roles available for this guild.")]
        public async Task SelfRoleList() {
            var roleList = "";
            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.ForEach(role => {
                var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
                roleList += $"**{currentRole?.Name}**\n";
            });

            await Reply(Context.Channel, CreateEmbed(Context, roleList));
        }
    }
}