using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Iam")]
        [Description("Gives yourself a role, if it is in the current guild's self role list.")]
        [Remarks("Usage: |prefix|iam {roleName}")]
        public async Task<VolteCommandResult> IamAsync([Remainder] string roleName)
        {
            var data = Db.GetData(Context.Guild);
            if (!data.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName)))
            {
                return BadRequest($"The role **{roleName}** isn't in the self roles list for this guild.");
            }

            var target = Context.Guild.Roles.FirstOrDefault(x => x.Name.EqualsIgnoreCase(roleName));
            if (target is null)
            {
                return BadRequest($"The role **{roleName}** doesn't exist in this guild.");
            }

            await Context.User.AddRoleAsync(target);
            return Ok($"Gave you the **{roleName}** role.");
        }
    }
}