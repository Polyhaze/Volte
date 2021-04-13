using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    [Group("SelfRole", "Sr")]
    public class SelfRoleModule : VolteModule
    {
        [Command, DummyCommand, Description("The command group for modifying SelfRoles.")]
        public Task<ActionResult> BaseAsync() => None();

        [Command("List", "L")]
        [Description("Gets a list of self roles available for this guild.")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            if (Context.GuildData.Extras.SelfRoles.IsEmpty())
                return BadRequest("No roles available to self-assign in this guild.");

            var roles = Context.GuildData.Extras.SelfRoles.Select(x =>
            {
                var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(x));
                return currentRole is null ? string.Empty : $"**{currentRole.Name}**";
            }).Where(x => !x.IsNullOrEmpty()).Join("\n");

            return Ok(Context.CreateEmbedBuilder(roles).WithTitle("Roles available to self-assign in this guild:"));
        }

        [Command("Add", "A")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleAddAsync([Remainder, Description("The role to add to the SelfRoles list.")]
            SocketRole role)
        {
            var target = Context.GuildData.Extras.SelfRoles.FirstOrDefault(x => x.EqualsIgnoreCase(role.Name));
            if (target is { })
                return BadRequest(
                    $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");

            Context.GuildData.Extras.SelfRoles.Add(role.Name);
            Db.Save(Context.GuildData);
            return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
        }

        [Command("Remove", "Rem", "R")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleRemoveAsync(
            [Remainder, Description("The role to remove from the SelfRoles list.")]
            SocketRole role)
        {
            if (!Context.GuildData.Extras.SelfRoles.ContainsIgnoreCase(role.Name))
                return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");

            Context.GuildData.Extras.SelfRoles.Remove(role.Name);
            Db.Save(Context.GuildData);
            return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
        }

        [Command("Clear", "Cl", "C")]
        [Description("Clears the self role list for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleClearAsync() => 
            Ok("Successfully cleared all Self Roles for this guild.").Apply(_ =>
                {
                    Context.GuildData.Extras.SelfRoles.Clear();
                    Db.Save(Context.GuildData);
                });
    }
}