using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Commands.Checks;

namespace Volte.Commands.Modules
{
    public sealed partial class SelfRoleModule
    {
        [Command("Add", "A")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("selfrole add {Role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleAddAsync([Remainder] SocketRole role)
        {
            var target = Context.GuildData.Extras.SelfRoles.FirstOrDefault(x => x.EqualsIgnoreCase(role.Name));
            if (target is null)
            {
                return BadRequest(
                    $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");
            }

            ModifyData(data =>
            {
                data.Extras.SelfRoles.Add(role.Name);
                return data;
            });
            return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
        }

        [Command("Remove", "R", "Rem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("selfrole remove {Role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleRemoveAsync([Remainder] SocketRole role)
        {
            if (Context.GuildData.Extras.SelfRoles.ContainsIgnoreCase(role.Name))
            {
                ModifyData(data =>
                {
                    data.Extras.SelfRoles.RemoveAt(Context.GuildData.Extras.SelfRoles.IndexOf(role.Name));
                    return data;
                });
                return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
            }

            return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");
        }

        [Command("Clear", "C", "Cl")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("selfrole clear")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleClearAsync()
        {
            ModifyData(data =>
            {
                data.Extras.SelfRoles.Clear();
                return data;
            });
            return Ok("Successfully cleared all Self Roles for this guild.");
        }
    }
}