using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("SelfRoleAdd", "SrA", "SrAdd")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [Remarks("selfroleadd {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleAddAsync([Remainder] SocketRole role)
        {
            var target = Context.GuildData.Extras.SelfRoles.FirstOrDefault(x => x.EqualsIgnoreCase(role.Name));
            if (target is null)
            {
                Context.GuildData.Extras.SelfRoles.Add(role.Name);
                Db.UpdateData(Context.GuildData);
                return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
            }

            return BadRequest(
                $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");
        }

        [Command("SelfRoleRemove", "SrR", "SrRem")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [Remarks("selfrole remove {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleRemoveAsync([Remainder] SocketRole role)
        {

            if (Context.GuildData.Extras.SelfRoles.ContainsIgnoreCase(role.Name))
            {
                Context.GuildData.Extras.SelfRoles.Remove(role.Name);
                Db.UpdateData(Context.GuildData);
                return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
            }

            return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");
        }

        [Command("SelfRoleClear", "SrC", "SrClear", "SelfroleC")]
        [Description("Clears the self role list for this guild.")]
        [Remarks("selfroleclear")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleClearAsync()
        {
            Context.GuildData.Extras.SelfRoles.Clear();
            Db.UpdateData(Context.GuildData);
            return Ok("Successfully cleared all Self Roles for this guild.");
        }
    }
}