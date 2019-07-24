using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("SelfRoleAdd", "SrA", "SrAdd")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfroleadd {role}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> SelfRoleAddAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);
            var target = data.Extras.SelfRoles.FirstOrDefault(x => x.EqualsIgnoreCase(role.Name));
            if (target is null)
            {
                data.Extras.SelfRoles.Add(role.Name);
                Db.UpdateData(data);
                return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
            }

            return BadRequest(
                $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");
        }

        [Command("SelfRoleRemove", "SrR", "SrRem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfrole remove {role}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> SelfRoleRemoveAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);

            if (data.Extras.SelfRoles.ContainsIgnoreCase(role.Name))
            {
                data.Extras.SelfRoles.Remove(role.Name);
                Db.UpdateData(data);
                return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
            }

            return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");
        }

        [Command("SelfRoleClear", "SrC", "SrClear", "SelfroleC")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("Usage: |prefix|selfroleclear")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> SelfRoleClearAsync()
        {
            var data = Db.GetData(Context.Guild);
            data.Extras.SelfRoles.Clear();
            Db.UpdateData(data);
            return Ok("Successfully cleared all Self Roles for this guild.");
        }
    }
}