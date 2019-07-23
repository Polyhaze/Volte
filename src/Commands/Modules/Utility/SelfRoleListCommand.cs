using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("Usage: |prefix|selfrolelist")]
        public Task<VolteCommandResult> SelfRoleListAsync()
        {
            var data = Db.GetData(Context.Guild);
            if (data.Extras.SelfRoles.Count > 0)
            {
                var roleList = data.Extras.SelfRoles.Select(x =>
                    {
                        var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x));
                        if (currentRole is null) return "";
                        return $"**{currentRole.Name}**";
                    })
                    .Where(x => x != string.Empty).Join("\n");

                return Ok(roleList);
            }

            return BadRequest("No roles available to self-assign in this guild.");
        }
    }
}