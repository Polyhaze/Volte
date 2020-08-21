using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Iam")]
        [Description("Gives yourself a role, if it is in the current guild's self role list.")]
        [Remarks("iam {String}")]
        public async Task<ActionResult> IamAsync([Remainder]DiscordRole role)
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
            {
                return BadRequest("This guild does not have any roles you can give yourself.");
            }
            
            if (!Context.GuildData.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(role.Name)))
            {
                return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");
            }

            var target = Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.EqualsIgnoreCase(role.Name)).Value;
            if (target is null)
            {
                return BadRequest($"The role **{role.Name}** doesn't exist in this guild.");
            }

            await Context.Member.GrantRoleAsync(target);
            return Ok($"Gave you the **{role.Name}** role.");
        }
    }
}