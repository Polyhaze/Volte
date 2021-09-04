using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    [Group("SelfRole", "Sr")]
    public class SelfRoleModule : VolteModule
    {
        [Command, DummyCommand, Description("The command group for modifying SelfRoles.")]
        public async Task<ActionResult> BaseAsync() =>
            Ok(await CommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

        [Command("List", "Ls", "L")]
        [Description("Gets a list of self roles available for this guild.")]
        public Task<ActionResult> SelfRoleListAsync()
        {
            if (Context.GuildData.Extras.SelfRoleIds.IsEmpty())
                return BadRequest("No roles available to self-assign in this guild.");

            var roles = Context.GuildData.Extras.SelfRoleIds.Select(x =>
                    Context.Guild.GetRole(x)?.Name ?? string.Empty)
                .Where(x => !x.IsNullOrEmpty())
                .Select(Format.Bold)
                .Join("\n");

            return Ok(Context.CreateEmbedBuilder(roles).WithTitle("Roles available to self-assign in this guild:"));
        }

        [Command("Add", "A")]
        [Description("Adds a role to the list of self roles for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleAddAsync(
            [Remainder, Description("The role to add to the SelfRoles list.")] SocketRole role)
        {
            if (Context.GuildData.Extras.SelfRoleIds.Contains(role.Id))
                return BadRequest($"The role **{role.Name}** is already in the Self Roles list for this guild!");

            Context.Modify(data => data.Extras.SelfRoleIds.Add(role.Id));
            return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
        }

        [Command("Remove", "Rem", "R")]
        [Description("Removes a role from the list of self roles for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleRemoveAsync(
            [Remainder, Description("The role to remove from the SelfRoles list.")]
            SocketRole role)
        {
            if (!Context.GuildData.Extras.SelfRoleIds.Contains(role.Id))
                return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");

            Context.Modify(data => data.Extras.SelfRoleIds.Remove(role.Id));
            return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
        }

        [Command("Clear", "Cl", "C")]
        [Description("Clears the self role list for this guild.")]
        [RequireGuildAdmin]
        public Task<ActionResult> SelfRoleClearAsync()
        {
            Context.Modify(data => data.Extras.SelfRoleIds.Clear());
            return Ok("Successfully cleared all Self Roles for this guild.");
        }
    }
}