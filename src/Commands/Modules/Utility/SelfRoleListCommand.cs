using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("SelfRoleList", "Srl")]
        [Description("Gets a list of self roles available for this guild.")]
        [Remarks("selfrolelist")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
                return BadRequest("No roles available to self-assign in this guild.");

            var roles = Context.GuildData.Extras.SelfRoles.Select(x =>
                {
                    var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x));
                    return currentRole is null ? "" : $"**{currentRole.Name}**";
                }).Where(x => !x.IsNullOrEmpty()).Join("\n");

            return Ok(Context.CreateEmbedBuilder(roles).WithTitle("Roles available to self-assign in this guild:"));
        }
    }
}