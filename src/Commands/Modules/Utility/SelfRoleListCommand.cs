using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("Usage: |prefix|selfrolelist")]
        public async Task SelfRoleListAsync()
        {
            var roleList = string.Empty;
            var data = Db.GetData(Context.Guild);
            if (config.SelfRoles.Count > 0)
            {
                foreach (var role in config.SelfRoles)
                {
                    var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
                    if (currentRole is null) continue;
                    roleList += $"**{currentRole.Name}**\n";
                }
                await Context.CreateEmbed(roleList).SendToAsync(Context.Channel);
            }
            else
            {
                roleList = "No roles available to self-assign in this guild.";
                await Context.CreateEmbed(roleList).SendToAsync(Context.Channel);
            }
        }
    }
}