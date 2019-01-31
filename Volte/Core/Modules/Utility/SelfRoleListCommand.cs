using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("SelfRoleList"), Alias("Srl")]
        [Summary("Gets a list of self roles available for this guild.")]
        [Remarks("Usage: |prefix|selfrolelist")]
        public async Task SelfRoleList() {
            var roleList = string.Empty;
            var config = Db.GetConfig(Context.Guild);
            if (config.SelfRoles.Count > 0) {
                config.SelfRoles.ForEach(role => {
                    var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
                    roleList += $"**{currentRole?.Name}**\n";
                });
                await Reply(Context.Channel, CreateEmbed(Context, roleList));
            }
            else {
                roleList = "No roles available to self-assign in this guild.";
                await Reply(Context.Channel, CreateEmbed(Context, roleList));
            }

            
        }
    }
}