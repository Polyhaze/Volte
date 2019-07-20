using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("Usage: |prefix|selfrolelist")]
        public async Task<VolteCommandResult> SelfRoleListAsync()
        {
            var roleList = string.Empty;
            var data = Db.GetData(Context.Guild);
            if (data.Extras.SelfRoles.Count > 0)
            {
                roleList = data.Extras.SelfRoles.Select(x =>
                    {
                        var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x));
                        if (currentRole is null) return "";
                        return $"**{currentRole.Name}**";
                    })
                    .Where(x => x != string.Empty).Join("\n");

                return Ok(roleList);
            }
            else
            {
                return BadRequest("No roles available to self-assign in this guild.");
            }
        }
    }
}